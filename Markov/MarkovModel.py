# %% Import libraries and data
import numpy as np
import pandas as pd
import os

# Read Chord Collection file and dataset
df_good = pd.read_csv("data/smallerDataSetTchosen.csv")

# %% Generate n-grams and probabilities

chords_and_probs_dict = {}

# Generate n-grams for different n values (2 to 5)
for n in range(2, 6):
    n_grams_all = []
    n_grams_1_less_all = []
    
    # Process each column of df_good
    for column in df_good:
        prog = ' '.join(df_good[column].dropna().values)
        all_chords = prog.split()

        # Generate n-grams and (n-1)-grams
        n_grams = zip(*[all_chords[i:] for i in range(n)])
        n_grams = [" ".join(ngram) for ngram in n_grams]
        n_grams_all += n_grams

        n_grams_1_less = zip(*[all_chords[i:] for i in range(n-1)])
        n_grams_1_less = [" ".join(ngram) for ngram in n_grams_1_less]
        n_grams_1_less_all += n_grams_1_less

        # Calculate probabilities for (n-1)-grams
        for chords in set(n_grams_1_less):
            n_grams_with_current_chord = [n_gram for n_gram in n_grams_all if chords.split() == n_gram.split()[0:-1]]

            # Count occurrences of each n-gram
            n_gram_cc_set = set(n_grams_with_current_chord)
            count_appearance = {ngram: n_grams_with_current_chord.count(ngram) for ngram in n_gram_cc_set}

            # Convert counts to probabilities
            total_count = sum(count_appearance.values())
            for ngram in count_appearance.keys():
                count_appearance[ngram] /= total_count

            # Get top 2 chords with highest probabilities
            options = np.array([key.split()[-1] for key in count_appearance.keys()])
            probabilities = list(count_appearance.values())
            sorted_chords = options[np.argsort(probabilities)][::-1][:2]

            chords_and_probs_dict[chords] = sorted_chords

# Save the result to an Excel file
df_output = pd.DataFrame.from_dict(chords_and_probs_dict, orient='index')
df_output.rename(columns={'Unnamed: 0': 'query_chords'}, inplace=True)
df_output.to_excel("data/n_grams_2_probabilities.xlsx")

