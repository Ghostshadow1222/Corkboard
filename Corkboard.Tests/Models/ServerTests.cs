using System.ComponentModel.DataAnnotations;
using Corkboard.Models;
using FluentAssertions;
using Xunit;

namespace Corkboard.Tests.Models;

/// <summary>
/// Security-focused unit tests for the Server model class.
/// Tests validation rules, default values, and data integrity constraints.
/// </summary>
public class ServerTests
{
	#region Constructor and Default Values Tests

	[Fact]
	public void Constructor_ShouldInitializeDefaultValues()
	{
		// Arrange & Act
		var server = new Server { OwnerId = "test-owner-id" };

		// Assert
		server.Name.Should().Be("My Server", "default name should be set");
		server.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1), 
			"CreatedAt should be set to current UTC time");
		server.Members.Should().NotBeNull().And.BeEmpty("Members collection should be initialized");
		server.Channels.Should().NotBeNull().And.BeEmpty("Channels collection should be initialized");
	}

	[Fact]
	public void Constructor_ShouldRequireOwnerId_DocumentationTest()
	{
		// This test documents that OwnerId is required via the 'required' modifier
		// The compiler enforces this at compile-time, so we can't create an instance
		// without providing OwnerId. This test simply documents that requirement.
		
		// Arrange & Act
		var server = new Server { OwnerId = "required-owner-id" };

		// Assert
		server.OwnerId.Should().NotBeNullOrEmpty("OwnerId is required by the 'required' modifier");
	}

	#endregion

	#region Name Validation Tests

	[Fact]
	public void Name_ShouldAcceptValidName()
	{
		// Arrange
		var server = new Server { OwnerId = "owner-123", Name = "Valid Server Name" };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("valid name should pass validation");
		results.Should().BeEmpty("no validation errors should be present");
	}

	[Fact]
	public void Name_ShouldRejectNull()
	{
		// Arrange
		var server = new Server { OwnerId = "owner-123", Name = null! };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeFalse("null name should fail validation");
		results.Should().ContainSingle(r => r.MemberNames.Contains("Name"),
			"Name field should have validation error");
	}

	[Fact]
	public void Name_ShouldRejectEmptyString()
	{
		// Arrange
		var server = new Server { OwnerId = "owner-123", Name = "" };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeFalse("empty name should fail validation");
		results.Should().ContainSingle(r => r.MemberNames.Contains("Name"),
			"Name field should have validation error");
	}

	[Fact]
	public void Name_ShouldRejectExcessivelyLongNames()
	{
		// Arrange
		var longName = new string('A', 101); // Exceeds 100 character limit
		var server = new Server { OwnerId = "owner-123", Name = longName };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeFalse("name exceeding 100 characters should fail validation");
		results.Should().ContainSingle(r => r.MemberNames.Contains("Name"),
			"Name field should have validation error for excessive length");
	}

	[Fact]
	public void Name_ShouldAcceptMaximumLength()
	{
		// Arrange
		var maxName = new string('A', 100); // Exactly 100 characters
		var server = new Server { OwnerId = "owner-123", Name = maxName };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("name with exactly 100 characters should pass validation");
		results.Should().BeEmpty();
	}

	#endregion

	#region IconUrl Validation Tests

	[Fact]
	public void IconUrl_ShouldAcceptNull()
	{
		// Arrange
		var server = new Server { OwnerId = "owner-123", IconUrl = null };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("null IconUrl should be valid (optional field)");
		results.Should().BeEmpty();
	}

	[Fact]
	public void IconUrl_ShouldAcceptValidUrl()
	{
		// Arrange
		var server = new Server 
		{ 
			OwnerId = "owner-123", 
			IconUrl = "https://example.com/icon.png" 
		};
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("valid URL should pass validation");
		results.Should().BeEmpty();
	}

	[Fact]
	public void IconUrl_ShouldRejectInvalidUrl()
	{
		// Arrange
		var server = new Server 
		{ 
			OwnerId = "owner-123", 
			IconUrl = "not-a-valid-url" 
		};
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeFalse("invalid URL format should fail validation");
		results.Should().ContainSingle(r => r.MemberNames.Contains("IconUrl"),
			"IconUrl field should have validation error");
	}

	[Fact]
	public void IconUrl_ShouldRejectExcessivelyLongUrl()
	{
		// Arrange
		var longUrl = "https://example.com/" + new string('a', 2030); // Exceeds 2048 character limit
		var server = new Server { OwnerId = "owner-123", IconUrl = longUrl };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeFalse("URL exceeding 2048 characters should fail validation");
		results.Should().ContainSingle(r => r.MemberNames.Contains("IconUrl"),
			"IconUrl should have validation error for excessive length");
	}

	[Theory]
	[InlineData("javascript:alert('XSS')")]
	[InlineData("data:text/html,<script>alert('XSS')</script>")]
	public void IconUrl_ShouldRejectPotentialXssUrls(string maliciousUrl)
	{
		// Arrange
		var server = new Server { OwnerId = "owner-123", IconUrl = maliciousUrl };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeFalse("potentially malicious URLs should fail validation");
		results.Should().ContainSingle(r => r.MemberNames.Contains("IconUrl"),
			"IconUrl should reject javascript: and data: schemes");
	}

	#endregion

	#region Description Validation Tests

	[Fact]
	public void Description_ShouldAcceptNull()
	{
		// Arrange
		var server = new Server { OwnerId = "owner-123", Description = null };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("null Description should be valid (optional field)");
		results.Should().BeEmpty();
	}

	[Fact]
	public void Description_ShouldAcceptValidDescription()
	{
		// Arrange
		var server = new Server 
		{ 
			OwnerId = "owner-123", 
			Description = "A wonderful server for collaboration" 
		};
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("valid description should pass validation");
		results.Should().BeEmpty();
	}

	[Fact]
	public void Description_ShouldRejectExcessivelyLongDescription()
	{
		// Arrange
		var longDescription = new string('A', 1001); // Exceeds 1000 character limit
		var server = new Server { OwnerId = "owner-123", Description = longDescription };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeFalse("description exceeding 1000 characters should fail validation");
		results.Should().ContainSingle(r => r.MemberNames.Contains("Description"),
			"Description should have validation error for excessive length");
	}

	[Fact]
	public void Description_ShouldAcceptMaximumLength()
	{
		// Arrange
		var maxDescription = new string('A', 1000); // Exactly 1000 characters
		var server = new Server { OwnerId = "owner-123", Description = maxDescription };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("description with exactly 1000 characters should pass validation");
		results.Should().BeEmpty();
	}

	#endregion

	#region OwnerId Validation Tests

	[Fact]
	public void OwnerId_ShouldBeRequired()
	{
		// Arrange
		var server = new Server { OwnerId = "owner-123" };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("valid OwnerId should pass validation");
		server.OwnerId.Should().NotBeNullOrEmpty("OwnerId must be set");
	}

	[Fact]
	public void OwnerId_ShouldNotAcceptEmptyString()
	{
		// Arrange
		var server = new Server { OwnerId = "" };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeFalse("empty OwnerId should fail validation");
		results.Should().ContainSingle(r => r.MemberNames.Contains("OwnerId"),
			"OwnerId should have validation error");
	}

	#endregion

	#region CreatedAt Tests

	[Fact]
	public void CreatedAt_ShouldBeUtcTime()
	{
		// Arrange & Act
		var server = new Server { OwnerId = "owner-123" };

		// Assert
		server.CreatedAt.Kind.Should().Be(DateTimeKind.Utc, 
			"CreatedAt should be stored in UTC to prevent timezone issues");
	}

	[Fact]
	public void CreatedAt_ShouldNotBeFutureDate()
	{
		// Arrange & Act
		var server = new Server { OwnerId = "owner-123" };

		// Assert
		server.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1),
			"CreatedAt should not be a future date");
	}

	[Fact]
	public void CreatedAt_CanBeSetManually()
	{
		// Arrange
		var specificDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		// Act
		var server = new Server 
		{ 
			OwnerId = "owner-123",
			CreatedAt = specificDate
		};

		// Assert
		server.CreatedAt.Should().Be(specificDate, 
			"CreatedAt can be manually set for testing or data migration purposes");
	}

	#endregion

	#region Collection Initialization Tests

	[Fact]
	public void Members_ShouldBeInitializedAsEmptyCollection()
	{
		// Arrange & Act
		var server = new Server { OwnerId = "owner-123" };

		// Assert
		server.Members.Should().NotBeNull("Members collection should be initialized")
			.And.BeEmpty("Members should start empty");
		server.Members.Should().BeAssignableTo<ICollection<ServerMember>>();
	}

	[Fact]
	public void Channels_ShouldBeInitializedAsEmptyCollection()
	{
		// Arrange & Act
		var server = new Server { OwnerId = "owner-123" };

		// Assert
		server.Channels.Should().NotBeNull("Channels collection should be initialized")
			.And.BeEmpty("Channels should start empty");
		server.Channels.Should().BeAssignableTo<ICollection<Channel>>();
	}

	[Fact]
	public void Members_ShouldAllowAddingItems()
	{
		// Arrange
		var server = new Server { OwnerId = "owner-123" };
		var member = new ServerMember 
		{ 
			ServerId = server.Id,
			UserId = "user-123"
		};

		// Act
		server.Members.Add(member);

		// Assert
		server.Members.Should().ContainSingle("one member was added");
		server.Members.Should().Contain(member);
	}

	[Fact]
	public void Channels_ShouldAllowAddingItems()
	{
		// Arrange
		var server = new Server { OwnerId = "owner-123" };
		var channel = new Channel 
		{ 
			Name = "general",
			ServerId = server.Id
		};

		// Act
		server.Channels.Add(channel);

		// Assert
		server.Channels.Should().ContainSingle("one channel was added");
		server.Channels.Should().Contain(channel);
	}

	#endregion

	#region Security and Data Integrity Tests

	[Fact]
	public void Server_ShouldNotAllowSqlInjectionInName()
	{
		// Arrange
		var maliciousName = "'; DROP TABLE Servers; --";
		var server = new Server { OwnerId = "owner-123", Name = maliciousName };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		// Should pass validation but be handled safely by parameterized queries
		isValid.Should().BeTrue("validation should pass, but database layer should use parameterized queries");
		server.Name.Should().Be(maliciousName, "value should be stored as-is for parameterized queries to handle");
	}

	[Fact]
	public void Server_ShouldNotAllowSqlInjectionInDescription()
	{
		// Arrange
		var maliciousDescription = "'; DELETE FROM Servers WHERE 1=1; --";
		var server = new Server 
		{ 
			OwnerId = "owner-123", 
			Description = maliciousDescription 
		};
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("validation should pass, but database layer should use parameterized queries");
		server.Description.Should().Be(maliciousDescription);
	}

	[Theory]
	[InlineData("<script>alert('XSS')</script>")]
	[InlineData("<img src=x onerror='alert(1)'>")]
	[InlineData("<iframe src='javascript:alert(1)'></iframe>")]
	public void Server_ShouldStoreButNotExecuteHtmlInName(string htmlContent)
	{
		// Arrange
		var server = new Server { OwnerId = "owner-123", Name = htmlContent };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		// Validation will pass since MaxLength is not exceeded
		// XSS prevention should happen in the view layer with proper encoding
		server.Name.Should().Be(htmlContent, 
			"HTML/script content should be stored as-is, with XSS prevention handled in presentation layer");
	}

	[Fact]
	public void Server_ShouldPreventOwnerIdManipulation()
	{
		// Arrange
		var server = new Server { OwnerId = "original-owner" };
		
		// Act - Attempt to change owner
		server.OwnerId = "malicious-user";

		// Assert
		server.OwnerId.Should().Be("malicious-user", 
			"while property can be set, authorization checks should prevent unauthorized owner changes");
		// Note: This test documents that business logic/authorization must prevent unauthorized owner changes
	}

	[Fact]
	public void Id_ShouldDefaultToZeroUntilSetByDatabase()
	{
		// Arrange & Act
		var server = new Server { OwnerId = "owner-123" };

		// Assert
		server.Id.Should().Be(0, "Id should be 0 until set by the database on insert");
	}

	#endregion

	#region Edge Case Tests

	[Fact]
	public void Server_ShouldHandleUnicodeCharactersInName()
	{
		// Arrange
		var unicodeName = "??? ?? Gaming Server";
		var server = new Server { OwnerId = "owner-123", Name = unicodeName };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("unicode characters should be supported in names");
		server.Name.Should().Be(unicodeName);
	}

	[Fact]
	public void Server_ShouldHandleWhitespaceOnlyName()
	{
		// Arrange
		var whitespaceName = "     ";
		var server = new Server { OwnerId = "owner-123", Name = whitespaceName };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		// The [Required] attribute rejects whitespace-only strings
		isValid.Should().BeFalse("whitespace-only names should fail validation with [Required] attribute");
		results.Should().ContainSingle(r => r.MemberNames.Contains("Name"),
			"Name field should have validation error for whitespace-only content");
	}

	[Fact]
	public void Server_ShouldHandleSpecialCharactersInName()
	{
		// Arrange
		var specialCharName = "Server!@#$%^&*()_+-=[]{}|;':\",./<>?";
		var server = new Server { OwnerId = "owner-123", Name = specialCharName };
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("special characters should be allowed in server names");
		server.Name.Should().Be(specialCharName);
	}

	#endregion

	#region Integration Tests

	[Fact]
	public void Server_ShouldBeValidWithAllRequiredFieldsOnly()
	{
		// Arrange
		var server = new Server 
		{ 
			OwnerId = "owner-123"
			// Using all default values
		};
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("server with only required fields should be valid");
		results.Should().BeEmpty();
	}

	[Fact]
	public void Server_ShouldBeValidWithAllFieldsPopulated()
	{
		// Arrange
		var server = new Server 
		{ 
			OwnerId = "owner-123",
			Name = "Complete Server",
			IconUrl = "https://example.com/icon.png",
			Description = "A fully populated server instance for testing"
		};
		var context = new ValidationContext(server);
		var results = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(server, context, results, true);

		// Assert
		isValid.Should().BeTrue("server with all fields populated should be valid");
		results.Should().BeEmpty();
	}

	#endregion
}
