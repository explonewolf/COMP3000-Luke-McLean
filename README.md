# COMP3000-Luke-McLean
 
# Writing Tool

## Overview

Writing Tool is a Windows Forms application that provides text editing with spell checking, grammar correction, and a simple login/registration system.

## Features

- **Text Editing**: Edit text with customizable fonts and sizes.
- **Spell Checking**: Highlights misspelled words and provides suggestions.
- **Grammar Correction**: Uses a Python server with a machine learning model to correct grammar.
- **Login/Registration**: Allows users to register and log in with hashed passwords stored in `hash.txt`.

## Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/explonewolf/COMP3000-Luke-McLean/tree/main
   ```

2. **Build the Project**:
   - Open the solution in Visual Studio.
   - Build the project to restore NuGet packages.

3. **Prepare the Python Environment**:
   - Ensure Python is installed.
   - Install required Python packages:
     ```bash
     pip install tensorflow transformers
     ```

4. **Place Dictionary Files**:
   - Make sure `en_GB.aff` and `en_GB.dic` or your desired dictionaries are in the output directory.

## Usage

1. **Run the Application**:
   - Start the application from Visual Studio or the executable in the output directory.

2. **Login/Registration**:
   - Use the login form to register or log in.
   - Usernames and hashed passwords are stored in `hash.txt`.

3. **Spell Check**:
   - Right click on any highlighted text to get Spell Check using NHunspell

4. **Grammar Correction**:
   - Click the "Grammerfy" button to send text to the Python server for grammar correction.

## Python Server

- The Python server uses a Large Language Model for grammar correction.
- Ensure the server script `Server.py` is in the `bin/Debug` directory.



