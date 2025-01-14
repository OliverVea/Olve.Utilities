import subprocess

MODEL = "llama3.1:8b"
TIMEOUT = 24 * 60 * 60

def query_llm(prompt, print_to_console: bool = False) -> str:
    args = ["ollama", "run", MODEL]

    if print_to_console:
        print("Running LLM: ", args)
        print()
        print('---')
        print()

    # Use subprocess.Popen with utf-8 encoding for stdin
    process = subprocess.Popen(args, stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=False)

    output = []
    try:
        # Encode the prompt as utf-8 and write it to stdin
        process.stdin.write(prompt.encode('utf-8'))
        process.stdin.close()

        # Stream output line by line, decoding as utf-8
        for line in iter(process.stdout.readline, b''):
            decoded_line = line.decode('utf-8')
            if print_to_console:
                print(decoded_line, end="")  # Print to console without adding extra newlines
            output.append(decoded_line)

        # Wait for the process to complete
        process.stdout.close()
        process.wait(timeout=TIMEOUT)

    except subprocess.TimeoutExpired:
        process.kill()
        print("Error: LLM query timed out.")
        return ""

    if process.returncode != 0:
        error_message = process.stderr.read().decode('utf-8')
        print("Error running LLM: ", error_message)
        return ""

    return ''.join(output).strip()




def generate_commit_message():
    """Function to generate commit message"""
    # Ensure the encoding is explicitly set to utf-8
    diff_output = subprocess.check_output(['git', 'diff', '--cached'], text=True, encoding='utf-8')

    llm_prompt = f"""
    Below is a diff of all staged changes, coming from the command:

    ```
    {diff_output}
    ```

    Please generate a concise, one-line commit message for these changes.

    Your only output should be a single line containing the commit message.
    Do not include any other information.
    Do not format the line in any way other than plain text.

    If the changes contain breaking changes, please start the commit message with "(Breaking)".

    Examples:
    - User display name now defaults to "Anonymous" if not provided
    - Fixed bug where user could not log in with email address
    - (Breaking) Removed support for Python 2.7
    """

    return query_llm(llm_prompt, print_to_console=False).strip('´`\'"\n')

def read_input(prompt):
    """Function to read user input"""
    return input(prompt)

def gcm():
    """Main function for the git commit script"""
    print("Generating...")
    commit_message = generate_commit_message()

    while True:
        print("\nProposed commit message:")
        print(commit_message)

        choice = read_input("Do you want to (a)ccept, (e)dit, (r)egenerate, or (c)ancel? ").strip().lower()

        if choice == 'a':
            try:
                subprocess.check_call(['git', 'commit', '-m', commit_message])
                print("Changes committed successfully!")
                return 0
            except subprocess.CalledProcessError:
                print("Commit failed. Please check your changes and try again.")
                return 1

        elif choice == 'e':
            new_message = read_input("Enter your commit message: ").strip()
            if new_message:
                try:
                    subprocess.check_call(['git', 'commit', '-m', new_message])
                    print("Changes committed successfully with your message!")
                    return 0
                except subprocess.CalledProcessError:
                    print("Commit failed. Please check your message and try again.")
                    return 1

        elif choice == 'r':
            print("Regenerating commit message...")
            commit_message = generate_commit_message()

        elif choice == 'c':
            print("Commit cancelled.")
            return 1

        else:
            print("Invalid choice. Please try again.")

if __name__ == "__main__":
    gcm()