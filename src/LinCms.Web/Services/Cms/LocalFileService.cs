﻿using System;
using System.IO;
using System.Security.Cryptography;
using LinCms.Web.Models.Cms.Files;
using LinCms.Web.Services.Cms.Interfaces;
using LinCms.Zero.Common;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace LinCms.Web.Services.Cms
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IFreeSql _freeSql;
        private readonly IConfiguration _configuration;

        public LocalFileService(IWebHostEnvironment hostingEnv, IFreeSql freeSql, IConfiguration configuration)
        {
            _hostingEnv = hostingEnv;
            _freeSql = freeSql;
            _configuration = configuration;
        }

        public FileDto Upload(IFormFile file, int key = 0)
        {
            string domainUrl = _configuration["SITE_DOMAIN"];
            string fileDir = _configuration["FILE:STORE_DIR"];

            string md5 = LinCmsUtils.GetHash<MD5>(file.OpenReadStream());

            LinFile linFile = _freeSql.Select<LinFile>().Where(r => r.Md5 == md5 && r.Type == 1).OrderByDescending(r => r.CreateTime).First();

            if (linFile != null && File.Exists(Path.Combine(_hostingEnv.WebRootPath, fileDir, linFile.Path)))
            {
                return new FileDto
                {
                    Id = linFile.Id,
                    Key = "file_" + key,
                    Path = linFile.Path,
                    Url = domainUrl + "/" + fileDir + "/" + linFile.Path
                };
            }

            string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim().ToString();

            DateTime now = DateTime.Now;

            string newSaveName = Guid.NewGuid() + Path.GetExtension(filename);

            string savePath = Path.Combine(_hostingEnv.WebRootPath, fileDir, now.ToString("yyy/MM/dd"));

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            long len;

            using (FileStream fs = File.Create(Path.Combine(savePath, newSaveName)))
            {
                file.CopyTo(fs);
                len = fs.Length;
                fs.Flush();
            }

            long id;
            string path = Path.Combine(now.ToString("yyy/MM/dd"), newSaveName).Replace("\\", "/");
            if (linFile == null)
            {
                LinFile saveLinFile = new LinFile()
                {
                    Extension = Path.GetExtension(filename),
                    Md5 = md5,
                    Name = filename,
                    Path = path,
                    Type = 1,
                    CreateTime = DateTime.Now,
                    Size = len
                };
                id = _freeSql.Insert(saveLinFile).ExecuteIdentity();
            }
            else
            {
                _freeSql.Update<LinFile>(linFile.Id).Set(a => a.Path, path).ExecuteAffrows();
                id = linFile.Id;
            }

            return new FileDto
            {
                Id = (int)id,
                Key = "file_" + key,
                Path = path,
                Url = domainUrl + "/" + fileDir + "/" + path
            };

        }
    }
}
