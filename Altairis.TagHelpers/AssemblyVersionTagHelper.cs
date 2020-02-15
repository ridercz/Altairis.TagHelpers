using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace Altairis.TagHelpers {
    [HtmlTargetElement("assembly-version", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class AssemblyVersionTagHelper : TagHelper {

        public AssemblyVersionDisplayStyle Display { get; set; } = AssemblyVersionDisplayStyle.Revision;

        public DateTimeKind TimeKind { get; set; } = DateTimeKind.Utc;

        public string TimeFormat { get; set; } = "g";

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            output.TagName = string.Empty;

            switch (this.Display) {
                case AssemblyVersionDisplayStyle.BuildTime:
                    try {
                        var dt = new DateTime(
                            version.Major,          // year
                            version.Minor,          // month
                            version.Build,          // day
                            version.Revision / 100, // hour
                            version.Revision % 100, // minute
                            0,                      // second
                            this.TimeKind);
                        output.Content.Append(dt.ToString(this.TimeFormat));
                    } catch (Exception) {
                        output.Content.Append(version.ToString(4));
                    }
                    break;
                case AssemblyVersionDisplayStyle.Major:
                    output.Content.Append(version.ToString(1));
                    break;
                case AssemblyVersionDisplayStyle.Minor:
                    output.Content.Append(version.ToString(2));
                    break;
                case AssemblyVersionDisplayStyle.Build:
                    output.Content.Append(version.ToString(3));
                    break;
                case AssemblyVersionDisplayStyle.Revision:
                default:
                    output.Content.Append(version.ToString(4));
                    break;
            }
        }
    }

    public enum AssemblyVersionDisplayStyle {
        BuildTime = 0,
        Major = 1,
        Minor = 2,
        Build = 3,
        Revision = 4
    }
}
