#!/bin/bash

# Check if correct number of arguments are provided
if [ "$#" -ne 1 ]; then
    echo "Usage: $0 <directory>"
    exit 1
fi

# Directory containing TPTP files
directory="$1"

# Check if directory exists
if [ ! -d "$directory" ]; then
    echo "Directory $directory does not exist."
    exit 1
fi

# Create CSV file
csv_file="output_lochness_bmc.csv"
echo "File Name,Console Output" > "$csv_file"

# Process each TPTP file in the directory
for file in "$directory"/*.tptp
do
    # Get filename without extension
    filename=$(basename -- "$file")
    filename="${filename%.*}"

    # Skip if the file ends with _ladder.tptp or _safety.tptp or _initial.tptp
    if [[ $file == *_ladder.tptp || $file == *_safety.tptp || $file == *_initial.tptp || $file == *Ladder.tptp ]]; then
        continue
    fi

    # ladderfile="${filename}_ladder.tptp"
    initialfile="${filename}_initial.tptp"
    safetyfile="${filename}_safety.tptp"

    # copy over the corresopnding ladder, initial and safety files and rename them to Ladder.tptp, Initial.tptp and Safety.tptp respectively
    # mv -f "$ladderfile" "Ladder.tptp"
    mv -f "$directory/$initialfile" "$directory/Initial.tptp"
    mv -f "$directory/$safetyfile" "$directory/Safety.tptp"

    # Process file using z3_tptp script and capture console output
    console_output=$(z3_tptp "$file")
    
    echo "$filename, $console_output"

    # Append file name and console output to CSV
    echo "$filename,\"$console_output\"" >> "$csv_file"
done

echo "CSV file generated: $csv_file"

