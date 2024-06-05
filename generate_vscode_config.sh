#!/bin/bash

workspaceFolder=$(pwd)

function get_project_paths {
    find "$1" -type d -exec find {} -maxdepth 1 -name "*.csproj" \;
}

function generate_launch_json {
    local workspaceFolder=$1
    shift
    local projects=("$@")
    local launchJsonPath="$workspaceFolder/.vscode/launch.json"

    mkdir -p "$(dirname "$launchJsonPath")"

    echo "{" > "$launchJsonPath"
    echo '    "version": "0.2.0",' >> "$launchJsonPath"
    echo '    "configurations": [' >> "$launchJsonPath"

    for project in "${projects[@]}"; do
        projectName=$(basename "${project%.*}")
        projectDir=$(dirname "$project")
        dirName=$(basename "$projectDir")
        parentDir=$(basename "$(dirname "$projectDir")")
        binPath="\${workspaceFolder}/${projectDir#$workspaceFolder/}/bin/Debug/net8.0/$projectName.dll"
        cwdPath="\${workspaceFolder}/${projectDir#$workspaceFolder/}"

        echo '        {' >> "$launchJsonPath"
        echo '            "name": "'$parentDir'/'$dirName'/'$projectName'",' >> "$launchJsonPath"
        echo '            "type": "coreclr",' >> "$launchJsonPath"
        echo '            "request": "launch",' >> "$launchJsonPath"
        echo '            "preLaunchTask": "build '$projectName'",' >> "$launchJsonPath"
        echo '            "program": "'$binPath'",' >> "$launchJsonPath"
        echo '            "args": [],' >> "$launchJsonPath"
        echo '            "cwd": "'$cwdPath'",' >> "$launchJsonPath"
        echo '            "stopAtEntry": false,' >> "$launchJsonPath"
        echo '            "console": "internalConsole"' >> "$launchJsonPath"
        echo '        },' >> "$launchJsonPath"
    done

    # Remove the last comma
    sed -i '$ s/,$//' "$launchJsonPath"

    echo '    ]' >> "$launchJsonPath"
    echo '}' >> "$launchJsonPath"
}

function generate_tasks_json {
    local workspaceFolder=$1
    shift
    local projects=("$@")
    local tasksJsonPath="$workspaceFolder/.vscode/tasks.json"

    mkdir -p "$(dirname "$tasksJsonPath")"

    echo "{" > "$tasksJsonPath"
    echo '    "version": "2.0.0",' >> "$tasksJsonPath"
    echo '    "tasks": [' >> "$tasksJsonPath"

    for project in "${projects[@]}"; do
        projectName=$(basename "${project%.*}")
        projectPath="\${workspaceFolder}/${project#$workspaceFolder/}"

        echo '        {' >> "$tasksJsonPath"
        echo '            "label": "build '$projectName'",' >> "$tasksJsonPath"
        echo '            "command": "dotnet",' >> "$tasksJsonPath"
        echo '            "type": "process",' >> "$tasksJsonPath"
        echo '            "args": [' >> "$tasksJsonPath"
        echo '                "build",' >> "$tasksJsonPath"
        echo '                "'$projectPath'"' >> "$tasksJsonPath"
        echo '            ],' >> "$tasksJsonPath"
        echo '            "problemMatcher": "$msCompile"' >> "$tasksJsonPath"
        echo '        },' >> "$tasksJsonPath"
    done

    # Remove the last comma
    sed -i '$ s/,$//' "$tasksJsonPath"

    echo '    ]' >> "$tasksJsonPath"
    echo '}' >> "$tasksJsonPath"
}

function generate_settings_json {
    local workspaceFolder=$1
    shift
    local projects=("$@")
    local testProjectPaths=()

    for project in "${projects[@]}"; do
        if [[ $project == *".Test.csproj" ]]; then
            testProjectPaths+=("\${workspaceFolder}/${project#$workspaceFolder/}")
        fi
    done

    local settingsJsonPath="$workspaceFolder/.vscode/settings.json"
    mkdir -p "$(dirname "$settingsJsonPath")"

    echo "{" > "$settingsJsonPath"
    echo '    "dotnet-test-explorer.testProjectPath": "'$(IFS=','; echo "${testProjectPaths[*]}")'",' >> "$settingsJsonPath"
    echo '    "editor.formatOnSave": true,' >> "$settingsJsonPath"
    echo '    "editor.renderWhitespace": "all",' >> "$settingsJsonPath"
    echo '    "editor.tabSize": 4,' >> "$settingsJsonPath"
    echo '    "editor.wordWrap": "on",' >> "$settingsJsonPath"
    echo '    "files.trimTrailingWhitespace": true,' >> "$settingsJsonPath"
    echo '    "files.insertFinalNewline": true,' >> "$settingsJsonPath"
    echo '    "files.trimFinalNewlines": true' >> "$settingsJsonPath"
    echo '}' >> "$settingsJsonPath"
}

projects=($(get_project_paths "$workspaceFolder"))

generate_launch_json "$workspaceFolder" "${projects[@]}"
generate_tasks_json "$workspaceFolder" "${projects[@]}"
generate_settings_json "$workspaceFolder" "${projects[@]}"

echo "launch.json, tasks.json, and settings.json have been generated successfully."
