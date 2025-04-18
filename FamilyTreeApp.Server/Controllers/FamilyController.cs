﻿using FamilyTreeApp.Server.Core.Interfaces;
using FamilyTreeApp.Server.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FamilyTreeApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FamilyController : ControllerBase
    {
        private readonly IFamilyTreeService _familyTreeService;
        public FamilyController(IFamilyTreeService familyTreeService)
        {
            _familyTreeService = familyTreeService;
        }

        [HttpGet]
        [Route("GetFamilyTreeNodes")]
        public async Task<IActionResult> GetFamilyTreeNodes()
        {
            var data = await _familyTreeService.GetFamilyTreeNodesAsync();
            return Ok(data);
        }

        [HttpPost]
        [Route("UpdateFamilyTreeNodes")]
        public async Task<IActionResult> UpdateFamilyTreeNodes([FromBody] UpdateNodeArgsDTO updateNodeArgs)
        {
            var data = await _familyTreeService.UpdateFamilyTreeNodesAsync(updateNodeArgs);
            return Ok(data);
        }
    }
}
