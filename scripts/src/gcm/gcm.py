import subprocess

def generate_commit_message():
    """Function to generate commit message"""
    diff_output = subprocess.check_output(['git', 'diff', '--cached'], text=True)
    # Assuming you have an API or method to run the LLM. Replace this with the actual LLM call.
    llm_prompt = f"""
    Below is a diff of all staged changes, coming from the command:

    ```
    {diff_output}
    ```

    Please generate a concise, one-line commit message for these changes.
    """
    # Replace this with the LLM API call (e.g., requests.post() or similar).
    # For now, just return a placeholder.
    return "Generated commit message based on the diff"

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