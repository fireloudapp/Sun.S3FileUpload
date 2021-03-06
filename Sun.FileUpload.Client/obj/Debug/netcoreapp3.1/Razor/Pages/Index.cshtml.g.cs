#pragma checksum "C:\Test\Sun\Sun.S3FileUpload\Sun.FileUpload.Client\Pages\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f64f7bc60b1e353ffaa57726b5a2c63f858d6416"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(Sun.FileUpload.Client.Pages.Pages_Index), @"mvc.1.0.razor-page", @"/Pages/Index.cshtml")]
namespace Sun.FileUpload.Client.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Test\Sun\Sun.S3FileUpload\Sun.FileUpload.Client\Pages\_ViewImports.cshtml"
using Sun.FileUpload.Client;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f64f7bc60b1e353ffaa57726b5a2c63f858d6416", @"/Pages/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f153819c825f652fc70efc1d476ad452b8eb2ce8", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Index : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "C:\Test\Sun\Sun.S3FileUpload\Sun.FileUpload.Client\Pages\Index.cshtml"
  
    ViewData["Title"] = "Home page";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div class=\"text-center\">\r\n    <h1 class=\"display-4\">File Upload Client </h1>\r\n    <p>Learn about <a href=\"https://docs.microsoft.com/aspnet/core\">building Web apps with ASP.NET Core</a>.</p>\r\n</div>\r\n\r\n\r\n<div>\r\n");
            WriteLiteral(@"    <input type=""file"" id=""fileUpload"" accept=""image/*"" />
    <!-- to support All image
        ref: https://stackoverflow.com/questions/4328947/limit-file-format-when-using-input-type-file-->
    <button id=""uploadBTN"" type=""button"" value=""Upload"">Upload</button>
    <div id=""output""></div>
    <div class=""progress"">
        <div class=""progress-bar""></div>
    </div>
</div>
<script>

    $(function () {
        $('#uploadBTN').on('click', function () {

            var fd = new FormData();
            fd.append("""", fileUpload.files[0]);
            $.ajax({
                xhr: function () {
                    var xhr = new window.XMLHttpRequest();
                    xhr.upload.addEventListener(""progress"", function (evt) {
                        if (evt.lengthComputable) {
                            var percentComplete = ((evt.loaded / evt.total) * 100);
                            $("".progress-bar"").width(percentComplete + '%');
                            $("".progress-bar"").htm");
            WriteLiteral(@"l(percentComplete + '%');
                        }
                    }, false);
                    return xhr;
                },

                url: 'https://localhost:8083/UploadAPI/api/FileService/upload',
                type: 'POST',
                mimeType: ""multipart/form-data"",
                data: fd,
                crossDomain: true,
                success: function (data) {
                    $('#output').html(data);
                },
                beforeSend: function (request) {
                    request.setRequestHeader(""clientName"", ""amazon-santoyo"");
                    request.setRequestHeader(""fileName"", ""benny"");
                    request.setRequestHeader(""branchName"", ""store-a"");
                    $("".progress-bar"").width('0%');
                    $('#uploadStatus').html('<img src=""images/loading.gif""/>');
                },
                /*
                 Bucket name should conform with DNS requirements:
                    - Should not cont");
            WriteLiteral(@"ain uppercase characters
                    - Should not contain underscores (_)
                    - Should be between 3 and 63 characters long
                    - Should not end with a dash
                    - Cannot contain two, adjacent periods
                    - Cannot contain dashes next to periods (e.g., ""my-.bucket.com"" and ""my.-bucket"" are invalid)
                 */
                cache: false,
                contentType: false,
                processData: false
            });

        });
    });

</script>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IndexModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<IndexModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<IndexModel>)PageContext?.ViewData;
        public IndexModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591
