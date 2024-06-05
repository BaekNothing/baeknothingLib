#!/bin/bash

# Check if the folder name argument is provided
if [ -z "$1" ]; then
    echo "Usage: $0 <FolderName>"
    exit 1
fi

FOLDER_NAME=$1
SOLUTION_NAME="baeknothingLib"
SOLUTION_FILE="${SOLUTION_NAME}.sln"

# Create necessary directories
mkdir -p "${FOLDER_NAME}"

# Create a new class library targeting net8.0
cd "${FOLDER_NAME}"
dotnet new classlib -n Script --framework net8.0
dotnet new nunit -n Script.Test --framework net8.0

# Navigate to the test project
cd "Script.Test"
# Modify Script.Test.csproj to reference Script project
cat <<EOL > Script.Test.csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../Script/Script.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="NUnit" Version="3.13.3" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.1.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.4.0" />
  </ItemGroup>

</Project>
EOL

# Navigate back to the folder root
cd ../../

# Create the solution file if it doesn't exist
if [ ! -f "${SOLUTION_FILE}" ]; then
    dotnet new sln -n ${SOLUTION_NAME}
fi

# Add the projects to the solution
dotnet sln ${SOLUTION_FILE} add "${FOLDER_NAME}/Script/Script.csproj"
dotnet sln ${SOLUTION_FILE} add "${FOLDER_NAME}/Script.Test/Script.Test.csproj"

echo "Project and test project created and added to the solution."
