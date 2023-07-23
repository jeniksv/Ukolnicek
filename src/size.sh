#!/bin/bash

function get_file_size() {
  local file_path="$1"
  du -sb "$file_path" | cut -f1
}

files=( "Common/Assignment.cs" \
	"Common/TcpTransfer.cs" \
	"Common/Notifications.cs" \
	"Server/Program.cs" \
       	"Server/Server.cs" \
	"Server/TcpUser.cs" \
	"Client/Program.cs" \
       	"Client/Client.cs"\
	"Client/IClient.cs")

total_size=0

for file in "${files[@]}"; do
  size=$(get_file_size "$file")
  total_size=$((total_size + size))
done

total_size_human=$(echo "$total_size" | numfmt --to=iec)

echo "Total size: $total_size_human"

