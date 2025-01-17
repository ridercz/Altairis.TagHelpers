﻿using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Altairis.TagHelpers;

public class DicebearAvatarTagHelper(IOptions<DicebearAvatarOptions> options) : TagHelper {

    private readonly DicebearAvatarOptions options = options.Value ?? new();

    // Seed

    public string Seed { get; set; } = string.Empty;

    public bool? HashSeed { get; set; }

    // Configuration properties

    public string? ApiEndpointBase { get; set; }

    public string? Sprites { get; set; }

    public bool? Flip { get; set; }

    public int? Rotate { get; set; }

    public int? Scale { get; set; }

    public int? Radius { get; set; }

    public int? Size { get; set; }

    public string? BackgroundColor { get; set; }

    public int? TranslateX { get; set; }

    public int? TranslateY { get; set; }

    public string? CustomParams { get; set; }

    // Tag helper

    public override void Process(TagHelperContext context, TagHelperOutput output) {
        // Run parent code
        base.Process(context, output);

        // Update local values with options
        this.ApiEndpointBase ??= this.options.ApiEndpointBase;
        this.BackgroundColor ??= this.options.BackgroundColor;
        this.CustomParams ??= this.options.CustomParams;
        this.Flip ??= this.options.Flip;
        this.HashSeed ??= this.options.HashSeed;
        this.Radius ??= this.options.Radius;
        this.Rotate ??= this.options.Rotate;
        this.Scale ??= this.options.Scale;
        this.Size ??= this.options.Size;
        this.Sprites ??= this.options.Sprites;
        this.TranslateX ??= this.options.TranslateX;
        this.TranslateY ??= this.options.TranslateY;

        // Prepare output element
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
        // Construct base URL
        var sb = new StringBuilder($"{this.ApiEndpointBase}{this.Sprites}/svg?seed={this.GetSeedHash()}");

        // Add optional parameters
        if (this.Flip == true) sb.Append("flip=true&");
        if (this.Rotate > 0 && this.Rotate < 360) sb.Append($"rotate={this.Rotate}&");
        if (this.Scale >= 0 && this.Scale <= 200 && this.Scale != 100) sb.Append($"scale={this.Scale}&");
        if (this.Radius > 0 && this.Radius <= 50) sb.Append($"radius={this.Radius}&");
        if (this.Size > 0) sb.Append($"size={this.Size}&");
        if (!string.IsNullOrWhiteSpace(this.BackgroundColor)) sb.Append($"backgroundColor={HttpUtility.UrlEncode(this.BackgroundColor)}&");
        if (this.TranslateX >= -100 && this.TranslateX <= 100 && this.TranslateX != 0) sb.Append($"translateX={this.TranslateX}&");
        if (this.TranslateY >= -100 && this.TranslateY <= 100 && this.TranslateY != 0) sb.Append($"translateY={this.TranslateY}&");
        if (!string.IsNullOrWhiteSpace(this.CustomParams)) sb.Append(this.CustomParams);

        // Remove trailing query string separator
        var url = sb.ToString().TrimEnd('&');
        return url;
    }

    private string GetSeedHash() {
        if (this.HashSeed == false) return this.Seed;
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(this.Seed));
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

public class DicebearAvatarOptions {

    public string ApiEndpointBase { get; set; } = "https://api.dicebear.com/9.x/";

    public const int DefaultSize = 80;

    public bool HashSeed { get; set; } = true;

    public string Sprites { get; set; } = DicebearAvatarSprite.Identicon;

    public bool Flip { get; set; } = false;

    public int Rotate { get; set; } = 0;

    public int Scale { get; set; } = 100;

    public int Radius { get; set; } = 0;

    public int Size { get; set; } = DefaultSize;

    public string? BackgroundColor { get; set; }

    public int TranslateX { get; set; } = 0;

    public int TranslateY { get; set; } = 0;

    public string? CustomParams { get; set; }

}