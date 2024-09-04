using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UniTrade.Tools
{
    public static class StoreImage
    {
        public static async Task<string> SaveImageAsync(IFormFile file, string apiType)
        {
            // 检查文件是否为空
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("文件为空或无效");
            }

            // 根据API类型设置保存路径
            string directoryPath;
            switch (apiType)
            {
                case "cover":
                    directoryPath = @"C:\data\images\cover";
                    break;
                case "productDetails":
                    directoryPath = @"C:\data\images\productDetails";
                    break;
                default:
                    directoryPath = @"C:\data\images\others";
                    break;
            }

            // 确保目录存在
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 生成文件名并保存文件
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(directoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 返回相对URL
            return $"/uploads/{fileName}";
        }
    }
}


