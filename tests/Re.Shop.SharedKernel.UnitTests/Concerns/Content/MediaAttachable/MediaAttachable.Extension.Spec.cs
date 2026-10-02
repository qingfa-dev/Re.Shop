using SharedKernel.Concerns.Content.MediaAttachable;

namespace SharedKernel.UnitTests.Concerns.Content.MediaAttachable;

public class MediaAttachableExtensionSpec
{
    private sealed class TestMediaAttachable : IMediaAttachable<string>
    {
        public ICollection<string> Media { get; } = new List<string>();
    }

    private static Result<TestMediaAttachable, Error> Success(
        TestMediaAttachable entity)
    {
        return Result<TestMediaAttachable, Error>.Success(entity);
    }

    [Fact]
    public void AddMedia_NewMedia_ShouldAttachMedia()
    {
        var entity = new TestMediaAttachable();

        var result = Success(entity).AddMedia("photo.jpg");

        result.IsSuccess.ShouldBeTrue();
        entity.Media.ShouldHaveSingleItem()
            .ShouldBe("photo.jpg");
    }

    [Fact]
    public void AddMedia_DuplicateMedia_ShouldFailWithoutMutation()
    {
        var entity = new TestMediaAttachable();
        Success(entity).AddMedia("photo.jpg");

        var result = Success(entity).AddMedia("photo.jpg");

        result.IsFailure.ShouldBeTrue();
        result.Errors![0].Code.ShouldBe("media_attachable.media.duplicate");
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
        result.Errors![0].Code.ShouldBe("media_attachable.media.not_found");
        entity.Media.ShouldHaveSingleItem();
    }
}
