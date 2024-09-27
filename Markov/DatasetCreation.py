# %%
import os
import numpy as np
import pandas as pd
import re

# %% 
df = pd.read_csv('data/songSectionDataClean.csv')
selected_columns = df[['Name', 'Progression', 'NumSectionChords']]

#%% Repeat chords in each part of songs by the number in NumSectionChords

def repeat_progression(row):
    return ' '.join([row['Progression']] * row['NumSectionChords'])

selected_columns['New_Progression'] = selected_columns.apply(repeat_progression, axis=1)
selected_columns = selected_columns.drop('Progression', axis=1)
selected_columns = selected_columns.rename(columns={'New_Progression': 'Progression'})

#combine all progressions in the parts of each song
parsed_data = selected_columns.groupby('Name')['Progression'].apply(lambda x: ' '.join(x)).reset_index()

#remove "-" in progressions
parsed_data['Progression'] = parsed_data['Progression'].str.replace('-', ' ')
print(parsed_data)

# %% map from chord number to chord labels
chord_mapping = {
    'vi-I': 'Bbmaj7',
    'VI#': 'A7',
    'V#': 'Bb',
    'IV': 'F',
    'vii': 'Bdim',
    'VII': 'B',
    'iii': 'Em',
    'III': 'C',
    'ii': 'Dm',
    'II': 'D',
    'IV': 'F',  
    'vi': 'Am',
    'iv': 'Fm',
    'i#':'C#',
    'I-I-I': 'Cmaj9',
   'I-I': 'Cmaj7',
    'v': 'Gm',
    'VI' : 'A',
    'V': 'G',
    'I': 'C',
    'i': 'Cm'
}

pattern = '|'.join(re.escape(key) for key in chord_mapping.keys())

def replace_chords(match):
    return chord_mapping[match.group(0)]

parsed_data['Progression'] = parsed_data['Progression'].str.replace(pattern, replace_chords, regex=True)

print(parsed_data)

# %%
parsed_data.to_csv('data/allSongs.csv', index=False)

# %% to be removed 
songs_to_drop = ['Change', 'Everything Has Changed', 'Young Dumb & Broke', 'you Need to Calm Down', 'you Should See Me in a Crown']
parsed_data = parsed_data.drop([8,11,52,57, 58])
print(parsed_data)

# %%
parsed_data.to_csv('data/chosenSongs.csv', index=False)

# %%
df_copy = parsed_data.copy()
df = df_copy.copy()

# Split the Progression column by space to get individual chords
df['Progression'] = df['Progression'].str.split()

# Calculate the maximum number of chords in a song
max_chords = df['Progression'].str.len().max()

new_df = pd.DataFrame(df['Progression'].tolist()).T
new_df.columns = df['Name'].tolist()
print(new_df)

# %%
new_df.to_csv('data/chosenSongs.csv', index=False)

# %%
# Rename the columns to 'chords'
new_df.columns = ['chords'] * len(new_df.columns)

# %%
