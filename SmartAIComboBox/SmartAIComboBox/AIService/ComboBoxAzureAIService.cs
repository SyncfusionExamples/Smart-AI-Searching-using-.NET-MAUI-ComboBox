using Azure.AI.OpenAI;
using Azure;
using System.Diagnostics;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;

namespace SmartAIComboBox.SmartAIComboBox
{
    public class ComboBoxAzureAIService
    {
        /// <summary>
        /// The EndPoint
        /// </summary>
        private const string endpoint = "https://YOUR_ACCOUNT.openai.azure.com/";

        /// <summary>
        /// The Deployment name
        /// </summary>
        private const string deploymentName = "GPT35Turbo";

        /// <summary>
        /// The API key
        /// </summary>
        private const string key = "";

        /// <summary>
        /// The chat completion service
        /// </summary>
        private IChatCompletionService? _chatCompletion;

        /// <summary>
        /// The kernel
        /// </summary>
        private Kernel? _kernel;

        /// <summary>
        /// The chat histroy
        /// </summary>
        private ChatHistory? _chatHistory;

        /// <summary>
        /// The credential valid field
        /// </summary>
        internal bool IsCredentialValid = false;

        /// <summary>
        /// The uri result field
        /// </summary>
        private Uri? _uriResult;


        public ComboBoxAzureAIService()
        {
            ValidateCredential();
        }

        #region Private Methods

        /// <summary>
        /// Validate Azure Credentials
        /// </summary>
        private async void ValidateCredential()
        {
            #region Azure OpenAI
            // Use below method for Azure Open AI
            this.GetAzureOpenAIKernel();
            #endregion

            #region Google Gemini
            // Use below method for Google Gemini
            //this.GetGoogleGeminiAIKernel();
            #endregion

            bool isValidUri = Uri.TryCreate(endpoint, UriKind.Absolute, out _uriResult)
                 && (_uriResult.Scheme == Uri.UriSchemeHttp || _uriResult.Scheme == Uri.UriSchemeHttps);

            if (!isValidUri || !endpoint.Contains("http") || string.IsNullOrEmpty(key) || key.Contains("API key") || string.IsNullOrEmpty(deploymentName) || deploymentName.Contains("deployment name"))
            {
                ShowAlertAsync();
                return;
            }
            try
            {
                if (_chatHistory != null && _chatCompletion != null)
                {
                    // test the semantic kernel with message.
                    _chatHistory.AddSystemMessage("Hello, Test Check");
                    await _chatCompletion.GetChatMessageContentAsync(chatHistory: _chatHistory, kernel: _kernel);
                }
            }
            catch (Exception)
            {
                // Handle any exceptions that indicate the credentials or endpoint are invalid.               
                ShowAlertAsync();
                return;
            }
            IsCredentialValid = true;
        }

        #region Azure OpenAI
        /// <summary>
        /// To get the Azure open ai kernel method
        /// </summary>
        private void GetAzureOpenAIKernel()
        {
            // Create the chat history
            _chatHistory = new ChatHistory();
            try
            {
                var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(deploymentName, endpoint, key);

                // Get the kernel from build
                _kernel = builder.Build();

                //Get the chat completions from kernel
                _chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion

        #region Goolge Gemini
        /// <summary>
        /// To get the google Gemini ai kermal
        /// </summary>
        private void GetGoogleGeminiAIKernel()
        {
            //            //First Add the below package to the application
            //            add package Microsoft.SemanticKernel.Connectors.Google

            //            // Create the chat history
            //            _chatHistory = new ChatHistory();
            //            #pragma warning disable SKEXP0070
            //            IKernelBuilder _kernelBuilder = Kernel.CreateBuilder();
            //            _kernelBuilder.AddGoogleAIGeminiChatCompletion(modelId: "NAME_OF_MODEL", apiKey: key);
            //            Kernel _kernel = _kernelBuilder.Build();

            //            //Get the chat completions from kernel
            //            _chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();
        }
        #endregion

        /// <summary>
        /// Show Alert Popup
        /// </summary>
        private async void ShowAlertAsync()
        {
            if (Application.Current?.MainPage != null && !IsCredentialValid)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "The Azure API key or endpoint is missing or incorrect. Please verify your credentials.", "OK");
            }
        }

        /// <summary>
        /// Gets a completion response from the AzureAI service based on the provided prompt.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GetCompletion(string prompt, CancellationToken cancellationToken)
        {
            if (_chatHistory != null && _chatCompletion != null)
            {
                if (_chatHistory.Count > 5)
                {
                    _chatHistory.RemoveRange(0, 2); //Remove the message history to avoid exceeding the token limit
                }
                // Add the user message to the options
                _chatHistory.AddUserMessage(prompt);
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var chatresponse = await _chatCompletion.GetChatMessageContentAsync(chatHistory: _chatHistory, kernel: _kernel);
                    cancellationToken.ThrowIfCancellationRequested();
                    _chatHistory.AddAssistantMessage(chatresponse.ToString());
                    return chatresponse.ToString();
                }
                catch (RequestFailedException ex)
                {
                    // Log the error message and rethrow the exception or handle it appropriately
                    Debug.WriteLine($"Request failed: {ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    // Handle other potential exceptions
                    Debug.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
            }
            return "";
        }

        #endregion
    }
}
