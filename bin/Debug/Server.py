import warnings
import os
warnings.filterwarnings("ignore")
import tensorflow as tf
#print(tf.__version__)  # For TensorFlow
from transformers import pipeline


def connect_to_grammar_llm():
    # Print the desired working directory
    #current_dir = os.path.join(os.getcwd(), "WindowsFormsApp1", "bin", "Debug")
    #print("Current Working Directory:", current_dir)

    # Define the path for output.txt
    #output_file_path = os.path.join(current_dir, 'output.txt')
    output_file_path = "output.txt"
    # Open output.txt and save its contents as a string
    try:
        with open(output_file_path, 'r') as file:
            input_text = file.read()
    except FileNotFoundError:
        print(f"Error: The file {output_file_path} does not exist.")
        return  # Exit the function if the file is not found

    # Use the input_text for further processing
    formatted_input = f"correct: {input_text}"
    
    # Use a different model for grammar correction
    grammar_model = pipeline("text2text-generation", model="sherif31/T5-Grammer-Correction",device=0)
    
    # Generate corrected text
    corrected_text = grammar_model(formatted_input, max_new_tokens=50)
    
    # Debugging output
    #print("Output Text:", output_text)  # Uncomment to see the output text
    #if corrected_text:
    print(corrected_text[0]['generated_text']) 





connect_to_grammar_llm()