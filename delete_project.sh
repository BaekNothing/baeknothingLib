#!/bin/bash

# Check if the folder name argument is provided
if [ -z "$1" ]; then
    echo "Usage: $0 <FolderName>"
    exit 1
fi

FOLDER_NAME=$1
SOLUTION_NAME="baeknothingLib"
SOLUTION_FILE="${SOLUTION_NAME}.sln"

# Remove the projects from the solution
dotnet sln ${SOLUTION_FILE} remove "${FOLDER_NAME}/Script/Script.csproj"
dotnet sln ${SOLUTION_FILE} remove "${FOLDER_NAME}/Script.Test/Script.Test.csproj"

# Remove the project directories
rm -rf "${FOLDER_NAME}/Script"
rm -rf "${FOLDER_NAME}/Script.Test"

rmdir --ignore-fail-on-non-empty "${FOLDER_NAME}"

echo "Projects removed and directories deleted."
