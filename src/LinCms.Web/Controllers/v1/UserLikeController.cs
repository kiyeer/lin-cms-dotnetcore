﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
using LinCms.Web.Models.v1.Articles;
using LinCms.Web.Models.v1.UserLikes;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    [Route("v1/user-like")]
    [ApiController]
    [Authorize]
    public class UserLikeController : ControllerBase
    {
        private readonly AuditBaseRepository<Article> _articleAuditBaseRepository;
        private readonly AuditBaseRepository<UserLike> _userLikeRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public UserLikeController( IMapper mapper, ICurrentUser currentUser, GuidRepository<TagArticle> tagArticleRepository, IFreeSql freeSql, IArticleService articleService, AuditBaseRepository<UserLike> userLikeRepository, AuditBaseRepository<Article> articleAuditBaseRepository)
        {
            _mapper = mapper;
            _currentUser = currentUser;
            _userLikeRepository = userLikeRepository;
            _articleAuditBaseRepository = articleAuditBaseRepository;
        }

        /// <summary>
        /// 用户点赞/取消点赞文章 
        /// </summary>
        /// <param name="createUpdateUserLike"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdateUserLikeDto createUpdateUserLike)
        {
            Expression<Func<UserLike, bool>> predicate = r => r.ArticleId == createUpdateUserLike.ArticleId && r.CreateUserId == _currentUser.Id;

            bool exist = _userLikeRepository.Select.Any(predicate);
            if (exist)
            {
                _userLikeRepository.Delete(predicate);
                _articleAuditBaseRepository.UpdateDiy.Set(r => r.LikedQuantity - 1).Where(r => r.Id == createUpdateUserLike.ArticleId).ExecuteAffrows();

                return ResultDto.Success("取消点赞成功");
            }

            UserLike userLike = _mapper.Map<UserLike>(createUpdateUserLike);
            
            _userLikeRepository.Insert(userLike);
            _articleAuditBaseRepository.UpdateDiy.Set(r=>r.LikedQuantity+1).Where(r=>r.Id==createUpdateUserLike.ArticleId).ExecuteAffrows();

            return ResultDto.Success("点赞成功");
        }
    }
}