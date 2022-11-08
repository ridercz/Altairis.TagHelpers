using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Altairis.TagHelpers;

public class DicebearAvatarTagHelper : TagHelper {
    private const int DEFAULT_SIZE = 80;

    // Seed

    public string Seed { get; set; } = string.Empty;

    public bool HashSeed { get; set; } = true;

    // Configuration properties

    public string Sprites { get; set; } = DicebearAvatarSprite.Identicon;

    public bool Flip { get; set; } = false;

    public int Rotate { get; set; } = 0;

    public int Scale { get; set; } = 100;

    public int Radius { get; set; } = 0;

    public int Size { get; set; } = DEFAULT_SIZE;

    public string? BackgroundColor { get; set; }

    public int TranslateX { get; set; } = 0;

    public int TranslateY { get; set; } = 0;

    public string? CustomParams { get; set; }

    // Tag helper

    public override void Process(TagHelperContext context, TagHelperOutput output) {
        base.Process(context, output);

        output.TagName = "img";
        output.TagMode = TagMode.SelfClosing;

        output.Attributes.Add("src", this.GetServiceUrl());
        if (this.Size > 0) {
            output.Attributes.Add("width", this.Size);
            output.Attributes.Add("height", this.Size);
        }
        if (context.AllAttributes["alt"] == null) output.Attributes.Add("alt", "Avatar");
    }

    // Helper methods

    private string GetServiceUrl() {
        var sb = new StringBuilder($"https://avatars.dicebear.com/api/{this.Sprites}/{this.GetSeedHash()}.svg?");
        if (this.Flip) sb.Append("flip=true&");
        if (this.Rotate > 0 && this.Rotate < 360) sb.Append($"rotate={this.Rotate}&");
        if (this.Scale >= 0 && this.Scale <= 200 && this.Scale != 100) sb.Append($"scale={this.Scale}&");
        if (this.Radius > 0 && this.Radius <= 50) sb.Append($"radius={this.Radius}&");
        if (this.Size > 0) sb.Append($"size={this.Size}&");
        if (!string.IsNullOrWhiteSpace(this.BackgroundColor)) sb.Append($"backgroundColor={HttpUtility.UrlEncode(this.BackgroundColor)}&");
        if (this.TranslateX >= -100 && this.TranslateX <= 100 && this.TranslateX != 0) sb.Append($"translateX={this.TranslateX}&");
        if (this.TranslateY >= -100 && this.TranslateY <= 100 && this.TranslateY != 0) sb.Append($"translateY={this.TranslateY}&");
        if (!string.IsNullOrWhiteSpace(this.CustomParams)) sb.Append(this.CustomParams);
        var url = sb.ToString().TrimEnd('?', '&');
        return url;
    }

    private string GetSeedHash() {
        if (!this.HashSeed) return this.Seed;

        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(this.Seed));
        var hashString = string.Join(string.Empty, hash.Select(x => x.ToString("X2")));
        return hashString;
    }

}

public static class DicebearAvatarSprite {
    public const string Male = "male";
    public const string Female = "female";
    public const string Human = "human";
    public const string Identicon = "identicon";
    public const string Initials = "initials";
    public const string Bottts = "bottts";
    public const string Avataaars = "avataaars";
    public const string JDenticon = "jdenticon";
    public const string Gridy = "gridy";
    public const string Micah = "micah";
}