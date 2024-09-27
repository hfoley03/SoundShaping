import csv
import json

csv_file = 'data/no_gaps_n_grams_2_probabilities_ch_format.csv'
json_file = 'data/no_gaps_n_grams_2_probabilities.json'

data = []

# read csv and populate data
with open(csv_file, newline='') as csvfile:
    reader = csv.DictReader(csvfile)
    headers = reader.fieldnames 
    print(f'Headers: {headers}')
    
    for row in reader:
        data.append({
            headers[0]: row[headers[0]],
            headers[1]: row[headers[1]],
            headers[2]: row[headers[2]]
        })

# write to json
with open(json_file, 'w') as jsonfile:
    json.dump(data, jsonfile, indent=4)
