using SharedKernel.Concerns.Content.MediaAttachable;

namespace SharedKernel.UnitTests.Concerns.Content.MediaAttachable;

public class MediaAttachableValidatorSpec
{
    private sealed class TestMediaAttachable : IMediaAttachable<string>
    {
        public ICollection<string> Media { get; } = new List<string>();
    }

    [Fact]
    public void ValidateAddMedia_ValidInput_ShouldSucceed()
    {
        var entity = new TestMediaAttachable();

        var result = MediaAttachableValidator.ValidateAddMedia(
            entity,
            "photo.jpg");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Media.ShouldBeEmpty();
    }

    [Fact]
    public void ValidateAddMedia_NullEntity_ShouldFailEntityRequired()
    {
        var result = MediaAttachableValidator.ValidateAddMedia(
            (TestMediaAttachable)null!,
            "photo.jpg");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("media_attachable.entity.required");
    }

    [Fact]
    public void ValidateAddMedia_DuplicateMedia_ShouldFailMediaDuplicate()
    {
        var entity = new TestMediaAttachable();
        entity.Media.Add("photo.jpg");

        var result = MediaAttachableValidator.ValidateAddMedia(
            entity,
            "photo.jpg");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("media_attachable.media.duplicate");
        entity.Media.ShouldHaveSingleItem();
    }

    [Fact]
    public void ValidateRemoveMedia_ValidInput_ShouldSucceed()
    {
        var entity = new TestMediaAttachable();
        entity.Media.Add("photo.jpg");

        var result = MediaAttachableValidator.ValidateRemoveMedia(
            entity,
            "photo.jpg");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeSameAs(entity);
        entity.Media.ShouldHaveSingleItem();
    }

    [Fact]
    public void ValidateRemoveMedia_NullEntity_ShouldFailEntityRequired()
    {
        var result = MediaAttachableValidator.ValidateRemoveMedia(
            (TestMediaAttachable)null!,
            "photo.jpg");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("media_attachable.entity.required");
    }

    [Fact]
    public void ValidateRemoveMedia_MissingMedia_ShouldFailMediaNotFound()
    {
        var entity = new TestMediaAttachable();
        entity.Media.Add("photo.jpg");

        var result = MediaAttachableValidator.ValidateRemoveMedia(
            entity,
            "video.mp4");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("media_attachable.media.not_found");
        entity.Media.ShouldHaveSingleItem();
    }
}
