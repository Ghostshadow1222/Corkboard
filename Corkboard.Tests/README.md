# Corkboard.Tests

This project contains unit tests for the Corkboard application, with a focus on security and data integrity.

## Test Structure

### Models/ServerTests.cs

Comprehensive security-focused tests for the `Server` model class covering:

#### 1. **Constructor and Default Values**
- Validates proper initialization of default values
- Ensures collections are initialized
- Verifies required fields are enforced

#### 2. **Name Validation**
- Tests for null/empty rejection
- Maximum length enforcement (100 characters)
- Boundary testing (exactly 100 characters)
- Unicode and special character support

#### 3. **IconUrl Validation**
- Optional field handling (null allowed)
- Valid URL format validation
- Maximum length enforcement (2048 characters)
- **XSS Prevention**: Rejects `javascript:` and `data:` URI schemes
- Invalid URL format rejection

#### 4. **Description Validation**
- Optional field handling
- Maximum length enforcement (1000 characters)
- Boundary testing

#### 5. **OwnerId Validation**
- Required field enforcement
- Empty string rejection
- Compile-time `required` modifier documentation

#### 6. **CreatedAt Timestamp**
- UTC timezone enforcement
- Future date prevention
- Manual override capability (for migrations/testing)

#### 7. **Collection Initialization**
- Members and Channels collections properly initialized
- Adding items to collections works correctly

#### 8. **Security Tests**

##### SQL Injection Protection
- Tests that malicious SQL in Name/Description fields are accepted by validation
- **Note**: Actual SQL injection protection relies on parameterized queries in the data access layer
- These tests document that validation doesn't reject SQL-like strings

##### XSS (Cross-Site Scripting) Protection
- Tests that HTML/script tags in Name field are stored as-is
- **Note**: XSS prevention should be handled in the presentation layer through proper HTML encoding
- IconUrl validation rejects dangerous URI schemes (`javascript:`, `data:`)

##### Authorization Considerations
- Documents that OwnerId can be changed programmatically
- **Note**: Business logic and authorization middleware should prevent unauthorized owner changes

#### 9. **Edge Cases**
- Unicode characters (emoji, non-Latin scripts)
- Whitespace-only strings (rejected by [Required] attribute)
- Special characters
- Boundary values for length constraints

#### 10. **Integration Tests**
- Valid server with only required fields
- Valid server with all fields populated

## Security Considerations

### What These Tests Cover
? Input validation (length, format, required fields)  
? XSS prevention in URL fields  
? Data integrity constraints  
? Default value initialization  
? UTC timestamp enforcement  

### What Requires Additional Protection
?? **SQL Injection**: Use parameterized queries/ORM (Entity Framework)  
?? **XSS in Output**: Use Razor's automatic HTML encoding or `@Html.Encode()`  
?? **Authorization**: Implement authorization policies to prevent unauthorized owner changes  
?? **CSRF**: Use anti-forgery tokens in forms  
?? **Mass Assignment**: Use ViewModels or `[Bind]` attributes in controllers  

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "FullyQualifiedName~ServerTests"

# Run tests matching a pattern
dotnet test --filter "DisplayName~Security"
```

## Dependencies

- **xUnit**: Test framework
- **FluentAssertions**: Readable assertion library
- **Corkboard**: Main application project

## Best Practices Demonstrated

1. **Arrange-Act-Assert (AAA)** pattern in all tests
2. **Descriptive test names** that explain what is being tested
3. **Security-first mindset** with dedicated security test section
4. **Boundary testing** for all length constraints
5. **Edge case coverage** (Unicode, special chars, whitespace)
6. **Clear documentation** of security responsibilities across layers

## Future Test Additions

Consider adding tests for:
- [ ] `Channel` model validation
- [ ] `Message` model validation
- [ ] `ServerMember` model validation
- [ ] `UserAccount` model validation
- [ ] Integration tests with DbContext
- [ ] Controller authorization tests
- [ ] API endpoint security tests
