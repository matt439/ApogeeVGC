using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using Xunit;

namespace ApogeeVGC.Tests.Events;

/// <summary>
/// Tests for EventHandlerInfo nullability validation
/// </summary>
public class EventHandlerInfoNullabilityTests
{
    [Fact]
    public void ValidateConfiguration_ThrowsWhenParameterNullabilityLengthMismatch()
    {
        // This would be caught at construction time in real code
        // Here we demonstrate what the validation catches
        
  var exception = Assert.Throws<InvalidOperationException>(() =>
 {
// Create a mock EventHandlerInfo with mismatched arrays
          var handlerInfo = new TestEventHandlerInfo
     {
                Id = EventId.BeforeMove,
    ExpectedParameterTypes = new[] { typeof(int), typeof(string), typeof(bool) },
        ParameterNullability = new[] { false, true } // Wrong length!
    };
 
            handlerInfo.TestValidateConfiguration();
        });
    
        Assert.Contains("ParameterNullability length (2)", exception.Message);
      Assert.Contains("does not match ExpectedParameterTypes length (3)", exception.Message);
    }
 
    [Fact]
    public void ValidateParameterNullability_ThrowsWhenNullPassedToNonNullableParameter()
    {
        var handlerInfo = new TestEventHandlerInfo
        {
      Id = EventId.BeforeMove,
       ExpectedParameterTypes = new[] { typeof(int), typeof(string) },
      ParameterNullability = new[] { false, true } // First param non-null, second nullable
        };
     
        // This should succeed - second param can be null
  handlerInfo.ValidateParameterNullability(new object?[] { 42, null });
        
        // This should fail - first param can't be null
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            handlerInfo.ValidateParameterNullability(new object?[] { null, "test" });
        });
  
        Assert.Contains("Parameter 0", exception.Message);
        Assert.Contains("is non-nullable but null was provided", exception.Message);
    }
    
    [Fact]
    public void ValidateConfiguration_SucceedsWhenArrayLengthsMatch()
    {
     var handlerInfo = new TestEventHandlerInfo
        {
   Id = EventId.BeforeMove,
            ExpectedParameterTypes = new[] { typeof(int), typeof(string), typeof(bool) },
          ParameterNullability = new[] { false, true, false } // Correct length!
        };
        
     // Should not throw
    handlerInfo.TestValidateConfiguration();
    }
    
    [Fact]
    public void ValidateConfiguration_SucceedsWhenParameterNullabilityIsNull()
    {
        var handlerInfo = new TestEventHandlerInfo
  {
          Id = EventId.BeforeMove,
          ExpectedParameterTypes = new[] { typeof(int), typeof(string) },
       ParameterNullability = null // null is allowed (means all non-nullable)
        };
        
        // Should not throw
        handlerInfo.TestValidateConfiguration();
    }
    
    // Test helper class that exposes protected method
    private class TestEventHandlerInfo : EventHandlerInfo
    {
   public void TestValidateConfiguration() => ValidateConfiguration();
    }
}
