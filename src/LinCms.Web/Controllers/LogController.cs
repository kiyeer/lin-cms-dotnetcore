﻿using LinCms.Web.Data;
using LinCms.Web.Domain;
using LinCms.Web.Models.Logs;
using LinCms.Web.Repositories;
using LinCms.Zero.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers
{
    [Route("cms/log")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly LinLogRepository _linLogRepository;

        public LogController(LinLogRepository linLogRepository)
        {
            _linLogRepository = linLogRepository;
        }

        /// <summary>
        /// 查询已经被记录过日志的用户（分页）
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
        public PagedResultDto<LinLog> GetLoggedUsers([FromQuery]PageDto pageDto)
        {
            return _linLogRepository.GetLoggedUsers(pageDto);
        }

        /// <summary>
        /// 查询日志信息（分页）
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public PagedResultDto<LinLog> GetLogs([FromQuery]LogSearchDto searchDto)
        {
            return _linLogRepository.GetLogUsers(searchDto);
        }

        /// <summary>
        /// 搜索日志信息（分页）
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public PagedResultDto<LinLog> SearchLogs([FromQuery]LogSearchDto searchDto)
        {
            return _linLogRepository.GetLogUsers(searchDto);
        }

    }
}
