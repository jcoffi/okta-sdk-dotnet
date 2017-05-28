﻿using FluentAssertions;
using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Okta.Sdk.UnitTests
{
    public class NestedResourceShould
    {
        [Fact]
        public void NotThrowForNonexistentNestedProperty()
        {
            var resource = new TestNestedResource();
            resource.Nested.Should().NotBeNull();
        }

        [Fact]
        public void AccessNestedProperties()
        {
            var data = new Dictionary<string, object>()
            {
                ["foo"] = "abc",
                ["bar"] = true,
                ["nested"] = new Dictionary<string, object>()
                {
                    ["foo"] = "nested is neet!",
                    ["Bar"] = false
                }
            };
            var changeTrackingDictionary = new ChangeTrackingDictionary(data, StringComparer.OrdinalIgnoreCase);

            var resource = new TestNestedResource(changeTrackingDictionary);

            resource.Should().NotBeNull();
            resource.Foo.Should().Be("abc");
            resource.Bar.Should().Be(true);

            resource.Nested.Should().NotBeNull();
            resource.Nested.Foo.Should().Be("nested is neet!");
            resource.Nested.Bar.Should().Be(false);
        }

        [Fact]
        public void TrackNestedModifications()
        {
            var data = new Dictionary<string, object>()
            {
                ["foo"] = "abc",
                ["bar"] = true,
                ["nested"] = new Dictionary<string, object>()
                {
                    ["foo"] = "nested is neet!",
                    ["Bar"] = false
                }
            };
            var changeTrackingDictionary = new ChangeTrackingDictionary(data, StringComparer.OrdinalIgnoreCase);
            var resource = new TestNestedResource(changeTrackingDictionary);
            resource.ModifiedData.Count.Should().Be(0);

            resource.Nested.Bar = true;
            resource.ModifiedData.Keys.Should().BeEquivalentTo("nested");
            resource.Nested.ModifiedData.Should().Contain(new KeyValuePair<string, object>("bar", true));
        }

        [Fact]
        public void InstantiateGraphWithModifications()
        {
            var resource = new TestNestedResource()
            {
                Nested = new TestNestedResource()
                {
                    Foo = "turtles all the way down?"
                }
            };

            resource.ModifiedData.Keys.Should().BeEquivalentTo("nested");
            resource.Nested.ModifiedData.Keys.Should().BeEquivalentTo("foo");
        }
    }
}