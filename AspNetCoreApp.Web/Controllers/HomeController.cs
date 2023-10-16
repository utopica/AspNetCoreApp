using AspNetCoreApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using System.Diagnostics;
using OpenAI.Interfaces;

namespace AspNetCoreApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOpenAIService _openAiService;

        public HomeController(ILogger<HomeController> logger, IOpenAIService openAiService)
        {
            _logger = logger;
            _openAiService = openAiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new HomeIndexViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomeIndexViewModel viewModel)
        {
            //Dall-e
            var imageResult = await _openAiService.Image.CreateImage(new ImageCreateRequest
            {
                Prompt = viewModel.Prompt,
                N = viewModel.ImageCount,
                Size = StaticValues.ImageStatics.Size.Size256,
                ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                User = "TestUser"
            });


            if (imageResult.Successful)
            {
                viewModel.ImageUrls = imageResult.Results.Select(r => r.Url).ToList();
            }

            return View();

            //Chat-gpt

            var completionResult = await _openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                    Messages = new List<ChatMessage>
                    {       
                        ChatMessage.FromUser("Where was it played?")
                    },
                    Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo,
                    MaxTokens = 2000
            });

            if (completionResult.Successful)
            {
                viewModel.ChatGptResponse = completionResult.Choices.First().Message.Content;
            }


            
        }
        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}