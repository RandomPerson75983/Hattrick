#!/bin/bash
# Block agents spawned with modes that bypass hooks
input=$(cat)
if echo "$input" | grep -qE '"mode"\s*:\s*"(bypassPermissions|acceptEdits|dontAsk|auto)"'; then
    echo "BLOCKED: Cannot spawn agents with modes that bypass hooks (bypassPermissions, acceptEdits, dontAsk, auto). Use default mode instead." >&2
    exit 2
fi
exit 0
