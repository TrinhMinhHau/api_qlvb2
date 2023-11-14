using CenIT.DegreeManagement.CoreAPI.Controllers.Account;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Resources;
using NPOI.HPSF;

namespace CenIT.DegreeManagement.CoreAPI.Processor.UploadFile
{
    public class FileService : IFileService
    {
        private IWebHostEnvironment _environment;
        private readonly ShareResource _localizer;

        public FileService(IWebHostEnvironment environment, ShareResource localizer)
        {
            _environment = environment;
            _localizer = localizer;
        }


        public Tuple<int, string> SaveFileImage(IFormFile imageFile, string folderName)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads" + "/" + folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var extension = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(extension))
                {
                    string msg = _localizer.GetFileFormatErrorMessage(allowedExtensions);
                    return new Tuple<int, string>(-1, msg);
                }

                var fileName = FileHelper.GetUniqueFileName(extension);
                var fileWithPath = Path.Combine(path, fileName);
                var stream = new FileStream(fileWithPath, FileMode.Create, access: FileAccess.ReadWrite);
                var url = $"/Resources/{folderName}/{fileName}";
                imageFile.CopyTo(stream);
                stream.Close();


                return new Tuple<int, string>(1, url);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, ex.Message);
            }
        }

        public Tuple<int, string> SaveImage(IFormFile imageFile, string folderName)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads" + "/" + folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var extension = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(extension))
                {
                    string msg = _localizer.GetFileFormatErrorMessage(allowedExtensions);
                    return new Tuple<int, string>(-1, msg);
                }

                var fileName = FileHelper.GetUniqueFileName(extension);
                var fileWithPath = Path.Combine(path, fileName);
                var stream = new FileStream(fileWithPath, FileMode.Create, access : FileAccess.ReadWrite);
                var url = $"/Resources/{folderName}/{fileName}";
                imageFile.CopyTo(stream);
                stream.Close();


                return new Tuple<int, string>(1, fileName);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, ex.Message);
            }
        }

        public Tuple<int, string> SaveFilePDFOrWorld(IFormFile imageFile, string folderName)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads" + "/" + folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var extension = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".pdf", ".doc", ".docx" };
                if (!allowedExtensions.Contains(extension))
                {
                    string msg = _localizer.GetFileFormatErrorMessage(allowedExtensions);
                    return new Tuple<int, string>(-1, msg);
                }

                var fileName = FileHelper.GetUniqueFileName(extension);
                var fileWithPath = Path.Combine(path, fileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                var url = $"/Resources/{folderName}/{fileName}";
                imageFile.CopyTo(stream);
                stream.Close();

                return new Tuple<int, string>(1, fileName);

            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, _localizer.GetUploadErrorMessage());
            }
        }

        public Tuple<int, string> SaveFile(IFormFile imageFile, string folderName)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads" + "/" + folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var extension = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".pdf", ".doc", ".docx" };
                if (!allowedExtensions.Contains(extension))
                {
                    string msg = _localizer.GetFileFormatErrorMessage(allowedExtensions);
                    return new Tuple<int, string>(-1, msg);
                }

                var fileName = FileHelper.GetUniqueFileName(extension);
                var fileWithPath = Path.Combine(path, fileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                var url = $"/Resources/{folderName}/{fileName}";
                imageFile.CopyTo(stream);
                stream.Close();

                return new Tuple<int, string>(1, url);

            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, _localizer.GetUploadErrorMessage());
            }
        }


        public bool DeleteImage(string imageFileName)
        {
            throw new NotImplementedException();
        }

        public Tuple<int, string> SaveFileExel(MemoryStream stream, string folderName, string fileName, string fileExtension)
        {
            try
            {
                var contentPath = _environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads" + "/" + folderName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = Path.Combine(path, fileName + fileExtension);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }

                var url = $"/Resources/{folderName}/{fileName}{fileExtension}";

                return new Tuple<int, string>(1, url);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, _localizer.GetUploadErrorMessage());
            }
        }
    }
}
