using BuildingBlocks.Domain.Concerns.Content.MediaAttachable;
namespace BuildingBlocks.Domain.Tests.Concerns.Content.MediaAttachable;

public class MediaAttachableExtensionTest
{
    private sealed class TestMediaAttachable : IMediaAttachable<string>
    {
        public ICollection<string> Media { get; } = new List<string>();
    }

    private static Result<TestMediaAttachable> Success(
        TestMediaAttachable entity)
    {
        return Result<TestMediaAttachable>.Success(entity);
    }

    [Fact]
    public void AddMedia_NewMedia_ShouldAttachMedia()
    {
        var entity = new TestMediaAttachable();

        var result = Success(entity).AddMedia("photo.jpg");

        result.IsSuccess.ShouldBeTrue();
        entity.Media.ShouldHaveSingleItem().ShouldBe("photo.jpg");
    }

    [Fact]
    public void AddMedia_DuplicateMedia_ShouldFailWithoutMutation()
    {
        var entity = new TestMediaAttachable();
        Success(entity).AddMedia("photo.jpg");

        var result = Success(entity).AddMedia("photo.jpg");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe(
            "MediaAttachable.Media.Duplicate");
        entity.Media.ShouldHaveSingleItem();
    }

    [Fact]
    public void RemoveMedia_AttachedMedia_ShouldDetachMedia()
    {
        var entity = new TestMediaAttachable();
        Success(entity).AddMedia("photo.jpg");

        var result = Success(entity).RemoveMedia("photo.jpg");

        result.IsSuccess.ShouldBeTrue();
        entity.Media.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveMedia_MissingMedia_ShouldFailWithoutMutation()
    {
        var entity = new TestMediaAttachable();
        Success(entity).AddMedia("photo.jpg");

        var result = Success(entity).RemoveMedia("video.mp4");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("MediaAttachable.Media.NotFound");
        entity.Media.ShouldHaveSingleItem();
    }

    [Fact]
    public void ValidateAddMedia_NullEntity_ShouldFailEntityRequired()
    {
        var result = MediaAttachableValidator.ValidateAddMedia(
            (TestMediaAttachable)null!,
            "photo.jpg");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("MediaAttachable.Entity.Required");
    }
}
