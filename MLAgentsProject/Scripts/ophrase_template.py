# ophrase_template.py

from jinja2 import Template as T

# Instructions for generating responses
RESPONSE_INSTR = (
    "Provide the task as plain JSON, no explanations or markdown.\n"
    "Return exactly 3 sentences in a JSON array.\n"
    "Only one JSON array, e.g., [\"sentence_a\", \"sentence_b\", ...]\n"
    "Sentences must be in double quotes.\n"
    "No markdown or code.\n"
    "Do not answer the input; only generate variations.\n"
    "No explanations; only a JSON array with 3 sentences.\n"
    "For spelling tasks, do not provide the correct spelling; only variations.\n"
    "Always include the word being spelled in the variations.\n"
    "Match the punctuation of the input text (e.g., questions, exclamations).\n"
    "If the User text is a question, generate 3 sentences rhetorically as a question."
)

# Instructions for proof validation
PROOF_INSTR = (
    "Provide the task as plain JSON, no explanations or markdown.\n"
    "Return exactly 3 sentences in a JSON array.\n"
    "Only one JSON array, e.g., [\"sentence_a\", \"sentence_b\", ...]\n"
    "Sentences must be in double quotes.\n"
    "No markdown or code.\n"
    "Do not answer the input; only generate variations.\n"
    "No explanations; only a JSON array with 3 sentences.\n"
    "For spelling tasks, do not provide the correct spelling; only variations.\n"
    "Always include the word being spelled in the variations.\n"
    "Match the punctuation of the input text (e.g., questions, exclamations).\n"
    "If the User text is a question, generate 3 sentences rhetorically as a question."
)

# System prompts
RESPONSE_SYS = "You are an expert writing system that generates {{ task }}s. Provide a {{ task }} for the following text in {{ lang }}."
PROOF_SYS = "You are an expert proofing system that validates {{ task }}s. Provide a {{ task }} for the following text in {{ lang }}."

# Prompt templates
PROMPT_TEMPLATE = (
    "System: {{ system }}\n"
    "Instructions: {{ instructions }}\n"
    "Example: {{ example }}\n"
    "User: {{ text }}\n"
    "System: Return only a JSON array of sentences. No explanations, only JSON array."
)

# Compiled templates
RESPONSE_TEMPLATE = T(PROMPT_TEMPLATE)
PROOF_TEMPLATE = T(PROMPT_TEMPLATE)

# Example tasks
TASKS = {
    "paraphrase": "What is 2 plus 2? What is the sum of 2 and 2?",
    "spelling": "How do you spell 'necessary'? How do you spell 'neccessary'?",
    "synonym": "What is another word for 'happy'? What is a synonym for 'joyful'?"
}
