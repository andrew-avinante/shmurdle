using Microsoft.AspNetCore.Mvc;
using server.Models;
using server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Controllers
{
    [ApiController]
    [Route("api")]
    public class WordController : ControllerBase
    {
        private readonly WordsService _wordService;

        public WordController(WordsService wordService)
        {
            _wordService = wordService;
        }

        [HttpGet("word-of-the-day")]
        public IActionResult GetWord(int length)
        {
            if(length < 3 || length > 10)
            {
                return BadRequest();
            }

            Words word = _wordService.GetWordOfTheDay(length, DateTime.Today.ToUniversalTime());
            if (word == null) {
                word = _wordService.GetNewWord(length);
                _wordService.Update(word.Id, word);
            }
            return Ok(word);
        }

        [HttpGet("check-word")]
        public IActionResult CheckWord(string word)
        {
            if (word == null)
            {
                return BadRequest();
            }

            Words foundWord = _wordService.GetWord(word);
            if (foundWord == null)
            {
                return Ok(false);
            }

            return Ok(true);
        }
    }
}
