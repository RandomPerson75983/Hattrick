#!/bin/bash
# PreToolUse hook: Enforce TDD workflow
# Blocks implementation .cs file edits until test files have been written first.
# Test files (in *Tests* paths) are always allowed — they're the first step in TDD.
input=$(cat)

# Extract file path from tool input JSON
file=$(echo "$input" | sed -n 's/.*"file_path" *: *"\([^"]*\).*/\1/p' | head -1)

# Only enforce TDD on .cs files
[[ "$file" != *.cs ]] && exit 0

# Test files are always allowed — writing tests IS the first TDD step
echo "$file" | grep -qi "Tests" && exit 0

# This is an implementation .cs file — check if tests were written first
STATE=".claude/hooks/tdd-session-state.txt"

if [ -f "$STATE" ] && grep -qi "Tests.*\.cs" "$STATE" 2>/dev/null; then
  # Tests exist in session state — implementation is allowed
  exit 0
fi

# No tests written yet — block and redirect to TDD workflow
cat >&2 <<'MSG'
TDD VIOLATION: You are editing implementation code before writing tests.

Follow the TDD workflow:
  1. @test-writer  — Write failing tests
  2. @test-writer  — Verify tests fail for the right reasons
  3. @coder         — Write implementation to make tests pass
  4. @verifier      — Run tests to confirm everything passes

Write your test files first, then retry this edit.
MSG
echo "Blocked file: $file" >&2
exit 2
