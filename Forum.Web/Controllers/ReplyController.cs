﻿using System.Threading.Tasks;
using System.Web.Mvc;
using ENode.Commanding;
using ENode.Infrastructure;
using Forum.Commands.Replies;
using Forum.QueryServices;
using Forum.Web.Extensions;
using Forum.Web.Models;
using Forum.Web.Services;
using ECommon.IO;

namespace Forum.Web.Controllers
{
    public class ReplyController : Controller
    {
        private readonly ICommandService _commandService;
        private readonly IContextService _contextService;
        private readonly IReplyQueryService _queryService;

        public ReplyController(ICommandService commandService, IContextService contextService, IReplyQueryService queryService)
        {
            _commandService = commandService;
            _contextService = contextService;
            _queryService = queryService;
        }

        [HttpGet]
        public ActionResult Find(string id, string option)
        {
            return Json(new
            {
                success = true,
                data = _queryService.FindDynamic(id, option)
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AjaxAuthorize]
        [AjaxValidateAntiForgeryToken]
        [AsyncTimeout(5000)]
        public async Task<ActionResult> Create(CreateReplyModel model)
        {
            AsyncTaskResult<CommandResult> asyncTaskResult = await _commandService.ExecuteAsync(
                new CreateReplyCommand(
                    model.PostId,
                    model.ParentId,
                    model.Body,
                    _contextService.CurrentAccount.AccountId), CommandReturnType.EventHandled);
            var result = asyncTaskResult.Data;
            if (result.Status == CommandStatus.Failed)
            {
                return Json(new { success = false, errorMsg = result.ErrorMessage });
            }

            return Json(new { success = true });
        }
        [HttpPost]
        [AjaxAuthorize]
        [AjaxValidateAntiForgeryToken]
        [AsyncTimeout(5000)]
        public async Task<ActionResult> Update(EditReplyModel model)
        {
            if (model.AuthorId != _contextService.CurrentAccount.AccountId)
            {
                return Json(new { success = false, errorMsg = "您不是回复的作者，不能编辑该回复。" });
            }

            AsyncTaskResult<CommandResult> asyncTaskResult = await _commandService.ExecuteAsync(new ChangeReplyBodyCommand(model.Id, model.Body), CommandReturnType.EventHandled);

            var result = asyncTaskResult.Data;
            if (result.Status == CommandStatus.Failed)
            {
                return Json(new { success = false, errorMsg = result.ErrorMessage });
            }

            return Json(new { success = true });
        }
    }
}