#!/bin/bash
# PostToolUse hook: Track edited files for TDD enforcement
# Logs every Edit/Write file path so the TDD enforce hook can verify
# that test files were written before implementation files.
input=$(cat)
file=$(echo "$input" | sed -n 's/.*"file_path" *: *"\([^"]*\).*/\1/p' | head -1)
if [ -n "$file" ]; then
  echo "$file" >> .claude/hooks/tdd-session-state.txt 2>/dev/null
fi
exit 0
