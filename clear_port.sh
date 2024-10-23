#!/bin/bash


# Function to print a step header
print_step() {
  echo ""
  echo "-------------------------------------------"
  echo "$1"
  echo "-------------------------------------------"
  echo ""
}

# Function to kill the process on a specified port
kill_port() {
  local PORT=$1
  print_step "Checking if anything is running on port $PORT..."
  PID=$(lsof -t -i:$PORT)

  if [ -n "$PID" ]; then
    echo "Process $PID is running on port $PORT. Terminating it now..."
    kill -9 $PID
    echo "Port $PORT has been cleared."
  else
    echo "No process running on port $PORT."
  fi
}

# Clear port 3000 (React frontend)
kill_port 3000

# Clear port 5000 (ASP.NET Core backend)
kill_port 5000

echo "All specified ports have been cleared."
