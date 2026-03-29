"""
PreToolUse hook: Code quality enforcement from review agents.
Covers agents: 01 (Magic Numbers), 03 (Thread Safety), 04 (Blazor),
05 (Test Quality), 08 (Architecture), 10/17 (Logic/Error Handling),
12 (Performance), 21 (Security).

Skips: non-.cs/.razor files, comment lines, const/readonly declarations.
"""
import json
import sys
import re

ALLOWED_MAGIC = {0, 1, -1, 2, 100, 1000, 0.0, 1.0, -1.0, 2.0, 100.0, 1000.0}


def is_cs(path):
    return path.endswith('.cs')


def is_test(path):
    return 'Test' in path


def is_razor(path):
    return path.endswith('.razor')


def file_matches(path, f):
    if f == "all_cs":
        return is_cs(path)
    if f == "non_test_cs":
        return is_cs(path) and not is_test(path)
    if f == "test_cs":
        return is_cs(path) and is_test(path)
    if f == "cs_and_razor":
        return is_cs(path) or is_razor(path)
    return False


# Line-level checks: (name, pattern, message, file_filter)
LINE_CHECKS = [
    # Agent 03: Thread Safety
    ("Random.Shared",
     r'\bRandom\.Shared\b',
     "Use IRandomProvider instead of Random.Shared [Agent 03]",
     "non_test_cs"),

    ("new Random()",
     r'\bnew\s+Random\s*\(',
     "Use IRandomProvider instead of new Random() [Agent 03]",
     "non_test_cs"),

    ("DateTime.Now",
     r'\bDateTime\.Now\b',
     "Never use DateTime.Now — use injected date parameter [Agent 03]",
     "non_test_cs"),

    ("Parallel.ForEach",
     r'\bParallel\.ForEach\b',
     "Use sequential foreach — Parallel.ForEach causes race conditions [Agent 03]",
     "all_cs"),

    # Agent 04: Blazor Patterns
    ("ToString w/o CultureInfo",
     r'\.ToString\s*\(\s*"[^"]*"\s*\)',
     "Add CultureInfo.InvariantCulture as second arg [Agent 04]",
     "cs_and_razor"),

    # Agent 05: Test Quality
    (": TestContext",
     r':\s*TestContext\b',
     "Use BunitContext instead of TestContext — bUnit v2 [Agent 05]",
     "test_cs"),

    ("RenderComponent<",
     r'\bRenderComponent\s*<',
     "Use Render<T>() instead of RenderComponent<T>() — bUnit v2 [Agent 05]",
     "test_cs"),

    # Agent 08: Architecture
    ("ServiceProvider.GetService",
     r'\bServiceProvider\s*\.\s*GetService',
     "Use constructor injection, not service locator [Agent 08]",
     "non_test_cs"),

    # Agent 10/17: Error Handling
    ("throw ex;",
     r'\bthrow\s+\w+\s*;',
     "Use 'throw;' to preserve stack trace, not 'throw ex;' [Agent 17]",
     "all_cs"),

    ("throw new Exception(",
     r'\bthrow\s+new\s+Exception\s*\(',
     "Use specific exception type (KeyNotFoundException, InvalidOperationException) [Agent 17]",
     "all_cs"),

    # Agent 12: Performance
    (".Count() vs 0",
     r'\.Count\(\)\s*[>!=<]+\s*0',
     "Use .Any() or !.Any() instead of .Count() comparison [Agent 12]",
     "all_cs"),

    # Agent 21: Security
    ("Console.Write",
     r'\bConsole\.Write(Line)?\s*\(',
     "Remove Console.Write — use proper logging [Agent 21]",
     "non_test_cs"),
]

# Content-level (multiline) checks
CONTENT_CHECKS = [
    ("empty catch",
     r'catch\s*(\([^)]*\))?\s*\{\s*\}',
     "Empty catch block swallows exceptions — log or rethrow [Agent 17]",
     "all_cs"),
]


def parse_number(s):
    try:
        return float(s)
    except ValueError:
        return None


def check_magic_numbers(cleaned, stripped):
    """Agent 01: Magic number detection."""
    hits = []

    # Comparisons: >, <, >=, <=, ==, !=
    for m in re.finditer(r'(?:[><!]=|[><]|==)\s*(-?\d+\.?\d*)[fFdDmM]?(?!\w)', cleaned):
        num = parse_number(m.group(1))
        if num is not None and num not in ALLOWED_MAGIC:
            hits.append(("magic: " + m.group(1), stripped[:100],
                         f"Extract {m.group(1)} to a named constant [Agent 01]"))

    # Multiplication/division: * N, / N
    for m in re.finditer(r'[*/]\s*(-?\d+\.?\d*)[fFdDmM]?(?!\w)', cleaned):
        num = parse_number(m.group(1))
        if num is not None and num not in ALLOWED_MAGIC:
            hits.append(("magic: " + m.group(1), stripped[:100],
                         f"Extract {m.group(1)} to a named constant [Agent 01]"))

    # TimeSpan/Duration: .FromDays(N), .FromMinutes(N)
    for m in re.finditer(r'\.From\w+\((\d+\.?\d*)', cleaned):
        num = parse_number(m.group(1))
        if num is not None and num not in ALLOWED_MAGIC:
            hits.append(("magic: " + m.group(1), stripped[:100],
                         f"Extract {m.group(1)} to a named constant [Agent 01]"))

    # Method arguments: SomeMethod(42), Assert.Equal(162, result)
    for m in re.finditer(r'[,(]\s*(-?\d+\.?\d*)[fFdDmM]?\s*[,)]', cleaned):
        num = parse_number(m.group(1))
        if num is not None and num not in ALLOWED_MAGIC:
            hits.append(("magic: " + m.group(1), stripped[:100],
                         f"Extract {m.group(1)} to a named constant [Agent 01]"))

    # Assignments: player.Age = 35;
    for m in re.finditer(r'(?<!=)=(?!=)\s*(-?\d+\.?\d*)[fFdDmM]?\s*;', cleaned):
        num = parse_number(m.group(1))
        if num is not None and num not in ALLOWED_MAGIC:
            hits.append(("magic: " + m.group(1), stripped[:100],
                         f"Extract {m.group(1)} to a named constant [Agent 01]"))

    return hits


def main():
    try:
        data = json.load(sys.stdin)
    except (json.JSONDecodeError, ValueError):
        sys.exit(0)

    tool_input = data.get('tool_input', {})
    file_path = tool_input.get('file_path', '')

    # Only check .cs and .razor files
    if not (is_cs(file_path) or is_razor(file_path)):
        sys.exit(0)

    code = tool_input.get('new_string', '') or tool_input.get('content', '')
    if not code:
        sys.exit(0)

    findings = []

    # --- Line-level checks ---
    for line in code.split('\n'):
        stripped = line.strip()
        if not stripped:
            continue
        if stripped.startswith('//') or stripped.startswith('/*') or stripped.startswith('*'):
            continue

        # Clean strings and trailing comments for pattern matching
        cleaned = re.sub(r'"[^"]*"', '""', stripped)
        cleaned = re.sub(r"'.'", "''", cleaned)
        cleaned = re.sub(r'//.*$', '', cleaned)

        is_const_line = bool(re.search(r'\b(const|readonly)\b', stripped))

        # Agent 01: Magic numbers (only non-test .cs, skip const/readonly)
        if is_cs(file_path) and not is_test(file_path) and not is_const_line:
            findings.extend(check_magic_numbers(cleaned, stripped))

        # All other line-level checks
        for name, pattern, message, file_filter in LINE_CHECKS:
            if not file_matches(file_path, file_filter):
                continue
            if re.search(pattern, cleaned):
                findings.append((name, stripped[:100], message))

    # --- Content-level checks (multiline) ---
    for name, pattern, message, file_filter in CONTENT_CHECKS:
        if not file_matches(file_path, file_filter):
            continue
        for _ in re.finditer(pattern, code):
            findings.append((name, "(multiline pattern)", message))

    if not findings:
        sys.exit(0)

    # Deduplicate
    seen = set()
    unique = []
    for name, context, message in findings:
        key = name + '|' + context
        if key not in seen:
            seen.add(key)
            unique.append((name, context, message))

    # Report
    print("CODE QUALITY ISSUES — fix before proceeding.", file=sys.stderr)
    print(f"File: {file_path}", file=sys.stderr)
    print("", file=sys.stderr)
    for name, context, message in unique[:8]:
        print(f"  [{name}] {message}", file=sys.stderr)
        if context != "(multiline pattern)":
            print(f"    in: {context}", file=sys.stderr)
    if len(unique) > 8:
        print(f"\n  ... and {len(unique) - 8} more issues", file=sys.stderr)
    print("", file=sys.stderr)
    print("Define constants as: const int/decimal/double DescriptiveName = value;", file=sys.stderr)
    sys.exit(2)


if __name__ == '__main__':
    main()
