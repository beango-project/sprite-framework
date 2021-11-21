// using System;
// using System.Collections.Generic;
// using System.Collections.Immutable;
// using System.ComponentModel;
// using System.Data;
// using System.Globalization;
// using System.IO;
// using System.Linq;
// using System.Runtime.CompilerServices;
// using System.Text;
// using System.Text.Encodings.Web;
// using System.Text.Json;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Caching.Memory;
// using Microsoft.Extensions.Options;
// using OpenIddict.Abstractions;
// using Sprite.Data.Repositories;
// using Sprite.Security.OpenIdConnect.Models;
// using Sprite.Security.OpenIdConnect.Stores;
// using SR = OpenIddict.Abstractions.OpenIddictResources;
//
// namespace Sprite.Security.OpenIdConnect.EntityFrameworkCore.Stores
// {
//     /// <summary>
//     /// Provides methods allowing to manage the applications stored in a database.
//     /// </summary>
//     /// <typeparam name="TContext">The type of the Entity Framework database context.</typeparam>
//     /// <typeparam name="TKey">The type of the entity primary keys.</typeparam>
//     public class OpenIdApplicationStore<TKey> :
//         OpenIdApplicationStore<OpenIdApplication<TKey>,
//             OpenIdAuthorization<TKey>,
//             OpenIdToken<TKey>, TKey>
//         where TKey : notnull, IEquatable<TKey>
//     {
//         public OpenIdApplicationStore(
//             IMemoryCache cache,
//             IOptionsMonitor<OpenIdConnectEntityFrameworkCoreOptions> options,
//             IRepository<OpenIdApplication<TKey>, TKey> applications,
//             IRepository<OpenIdAuthorization<TKey>, TKey> authorizations,
//             IRepository<OpenIdToken<TKey>, TKey> tokens)
//             : base(cache, options, applications, authorizations, tokens)
//         {
//         }
//     }
//
//     public class OpenIdApplicationStore<TApplication, TAuthorization, TToken, TKey> : IOpenIdApplicationStore<TApplication>
//         where TApplication : OpenIdApplication<TKey, TAuthorization, TToken>
//         where TAuthorization : OpenIdAuthorization<TKey, TApplication, TToken>
//         where TToken : OpenIdToken<TKey, TApplication, TAuthorization>
//         where TKey : notnull, IEquatable<TKey>
//     {
//         public OpenIdApplicationStore(
//             IMemoryCache cache,
//             IOptionsMonitor<OpenIdConnectEntityFrameworkCoreOptions> options,
//             IRepository<TApplication, TKey> applications,
//             IRepository<TAuthorization, TKey> authorizations,
//             IRepository<TToken, TKey> tokens)
//         {
//             Cache = cache;
//
//             Options = options;
//             Applications = applications;
//             Authorizations = authorizations;
//             Tokens = tokens;
//         }
//
//         /// <summary>
//         /// Gets the memory cache associated with the current store.
//         /// </summary>
//         protected virtual IMemoryCache Cache { get; }
//
//         protected IOptionsMonitor<OpenIdConnectEntityFrameworkCoreOptions> Options { get; }
//
//         /// <summary>
//         /// Gets the database context associated with the current store.
//         /// </summary>
//         // protected virtual TContext Context { get; }
//         protected IRepository<TApplication, TKey> Applications;
//
//         protected IRepository<TAuthorization, TKey> Authorizations;
//
//         protected IRepository<TToken, TKey> Tokens;
//
//         public bool AutoSaveChanges { get; set; }
//
//
//         public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
//         {
//             return (await Applications.GetAllAsync(cancellationToken)).LongCount();
//         }
//
//         public virtual async ValueTask<long> CountAsync<TResult>(Func<IQueryable<TApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken)
//         {
//             Check.NotNull(query, nameof(query));
//
//             return await Task.FromResult(query(Applications).LongCount());
//         }
//
//         public virtual async ValueTask CreateAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             await Applications.AddAsync(application, AutoSaveChanges, cancellationToken);
//         }
//
//         public virtual async ValueTask DeleteAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//             try
//             {
//                 await Authorizations.DeleteManyAsync(x => x.Application.Id.Equals(application.Id), AutoSaveChanges, cancellationToken);
//                 await Tokens.DeleteManyAsync(x => x.Application.Id.Equals(application.Id), AutoSaveChanges, cancellationToken);
//                 await Applications.DeleteAsync(application, AutoSaveChanges, cancellationToken);
//             }
//             catch (DBConcurrencyException exception)
//             {
//                 throw new OpenIddictExceptions.ConcurrencyException(SR.GetResourceString(SR.ID0239), exception);
//             }
//         }
//
//         public virtual async ValueTask<TApplication?> FindByIdAsync(string identifier, CancellationToken cancellationToken = default)
//         {
//             if (string.IsNullOrEmpty(identifier))
//             {
//                 throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
//             }
//
//             var key = ConvertIdentifierFromString(identifier);
//
//             return await Applications.GetAsync(x => x.Id.Equals(key), cancellationToken);
//         }
//
//         public virtual async ValueTask<TApplication?> FindByClientIdAsync(string identifier, CancellationToken cancellationToken = default)
//         {
//             if (string.IsNullOrEmpty(identifier))
//             {
//                 throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
//             }
//
//             return await Applications.GetAsync(x => x.ClientId == identifier, cancellationToken);
//         }
//
//         public virtual async IAsyncEnumerable<TApplication> FindByPostLogoutRedirectUriAsync(string address, CancellationToken cancellationToken = default)
//         {
//             if (string.IsNullOrEmpty(address))
//             {
//                 throw new ArgumentException(SR.GetResourceString(SR.ID0143), nameof(address));
//             }
//
//             var applications = await Applications.GetAllMergeAsync(x => x.RedirectUris!.Contains(address), cancellationToken);
//
//
//             foreach (var application in applications)
//             {
//                 var addresses = await GetPostLogoutRedirectUrisAsync(application, cancellationToken);
//                 if (addresses.Contains(address, StringComparer.Ordinal))
//                 {
//                     yield return application;
//                 }
//             }
//         }
//
//         public virtual async IAsyncEnumerable<TApplication> FindByRedirectUriAsync(string address, CancellationToken cancellationToken = default)
//         {
//             if (string.IsNullOrEmpty(address))
//             {
//                 throw new ArgumentException(SR.GetResourceString(SR.ID0143), nameof(address));
//             }
//
//             var applications = await Applications.GetAllMergeAsync(x => x.RedirectUris!.Contains(address), cancellationToken);
//
//             foreach (var application in applications)
//             {
//                 var addresses = await GetRedirectUrisAsync(application, cancellationToken);
//                 if (addresses.Contains(address, StringComparer.Ordinal))
//                 {
//                     yield return application;
//                 }
//             }
//         }
//
//         public virtual async ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<TApplication>, TState, IQueryable<TResult>> query, TState state,
//             CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(query, nameof(query));
//
//             return await Task.FromResult(query(Applications, state).FirstOrDefault());
//         }
//
//         public virtual ValueTask<string?> GetClientIdAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             return new ValueTask<string?>(application.ClientId);
//         }
//
//         public virtual ValueTask<string?> GetClientSecretAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             return new ValueTask<string?>(application.ClientSecret);
//         }
//
//         public virtual ValueTask<string?> GetClientTypeAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             return new ValueTask<string?>(application.Type);
//         }
//
//         public virtual ValueTask<string?> GetConsentTypeAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             return new ValueTask<string?>(application.ConsentType);
//         }
//
//         public virtual ValueTask<string?> GetDisplayNameAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             return new ValueTask<string?>(application.DisplayName);
//         }
//
//         public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (string.IsNullOrEmpty(application.DisplayNames))
//             {
//                 return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
//             }
//
//             // Note: parsing the stringified display names is an expensive operation.
//             // To mitigate that, the resulting object is stored in the memory cache.
//             var key = string.Concat("7762c378-c113-4564-b14b-1402b3949aaa", "\x1e", application.DisplayNames);
//             var names = Cache.GetOrCreate(key, entry =>
//             {
//                 entry.SetPriority(CacheItemPriority.High)
//                     .SetSlidingExpiration(TimeSpan.FromMinutes(1));
//
//                 using var document = JsonDocument.Parse(application.DisplayNames);
//                 var builder = ImmutableDictionary.CreateBuilder<CultureInfo, string>();
//
//                 foreach (var property in document.RootElement.EnumerateObject())
//                 {
//                     var value = property.Value.GetString();
//                     if (string.IsNullOrEmpty(value))
//                     {
//                         continue;
//                     }
//
//                     builder[CultureInfo.GetCultureInfo(property.Name)] = value;
//                 }
//
//                 return builder.ToImmutable();
//             });
//
//             return new ValueTask<ImmutableDictionary<CultureInfo, string>>(names);
//         }
//
//         public virtual ValueTask<string?> GetIdAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             return new ValueTask<string?>(ConvertIdentifierToString(application.Id));
//         }
//
//         public virtual ValueTask<ImmutableArray<string>> GetPermissionsAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (string.IsNullOrEmpty(application.Permissions))
//             {
//                 return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
//             }
//
//             // Note: parsing the stringified permissions is an expensive operation.
//             // To mitigate that, the resulting array is stored in the memory cache.
//             var key = string.Concat("0347e0aa-3a26-410a-97e8-a83bdeb21a1f", "\x1e", application.Permissions);
//             var permissions = Cache.GetOrCreate(key, entry =>
//             {
//                 entry.SetPriority(CacheItemPriority.High)
//                     .SetSlidingExpiration(TimeSpan.FromMinutes(1));
//
//                 using var document = JsonDocument.Parse(application.Permissions);
//                 var builder = ImmutableArray.CreateBuilder<string>(document.RootElement.GetArrayLength());
//
//                 foreach (var element in document.RootElement.EnumerateArray())
//                 {
//                     var value = element.GetString();
//                     if (string.IsNullOrEmpty(value))
//                     {
//                         continue;
//                     }
//
//                     builder.Add(value);
//                 }
//
//                 return builder.ToImmutable();
//             });
//
//             return new ValueTask<ImmutableArray<string>>(permissions);
//         }
//
//         public virtual ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (string.IsNullOrEmpty(application.PostLogoutRedirectUris))
//             {
//                 return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
//             }
//
//             // Note: parsing the stringified addresses is an expensive operation.
//             // To mitigate that, the resulting array is stored in the memory cache.
//             var key = string.Concat("fb14dfb9-9216-4b77-bfa9-7e85f8201ff4", "\x1e", application.PostLogoutRedirectUris);
//             var addresses = Cache.GetOrCreate(key, entry =>
//             {
//                 entry.SetPriority(CacheItemPriority.High)
//                     .SetSlidingExpiration(TimeSpan.FromMinutes(1));
//
//                 using var document = JsonDocument.Parse(application.PostLogoutRedirectUris);
//                 var builder = ImmutableArray.CreateBuilder<string>(document.RootElement.GetArrayLength());
//
//                 foreach (var element in document.RootElement.EnumerateArray())
//                 {
//                     var value = element.GetString();
//                     if (string.IsNullOrEmpty(value))
//                     {
//                         continue;
//                     }
//
//                     builder.Add(value);
//                 }
//
//                 return builder.ToImmutable();
//             });
//
//             return new ValueTask<ImmutableArray<string>>(addresses);
//         }
//
//         public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (string.IsNullOrEmpty(application.Properties))
//             {
//                 return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
//             }
//
//             // Note: parsing the stringified properties is an expensive operation.
//             // To mitigate that, the resulting object is stored in the memory cache.
//             var key = string.Concat("2e3e9680-5654-48d8-a27d-b8bb4f0f1d50", "\x1e", application.Properties);
//             var properties = Cache.GetOrCreate(key, entry =>
//             {
//                 entry.SetPriority(CacheItemPriority.High)
//                     .SetSlidingExpiration(TimeSpan.FromMinutes(1));
//
//                 using var document = JsonDocument.Parse(application.Properties);
//                 var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();
//
//                 foreach (var property in document.RootElement.EnumerateObject())
//                 {
//                     builder[property.Name] = property.Value.Clone();
//                 }
//
//                 return builder.ToImmutable();
//             });
//
//             return new ValueTask<ImmutableDictionary<string, JsonElement>>(properties);
//         }
//
//         public virtual ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (string.IsNullOrEmpty(application.RedirectUris))
//             {
//                 return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
//             }
//
//             // Note: parsing the stringified addresses is an expensive operation.
//             // To mitigate that, the resulting array is stored in the memory cache.
//             var key = string.Concat("851d6f08-2ee0-4452-bbe5-ab864611ecaa", "\x1e", application.RedirectUris);
//             var addresses = Cache.GetOrCreate(key, entry =>
//             {
//                 entry.SetPriority(CacheItemPriority.High)
//                     .SetSlidingExpiration(TimeSpan.FromMinutes(1));
//
//                 using var document = JsonDocument.Parse(application.RedirectUris);
//                 var builder = ImmutableArray.CreateBuilder<string>(document.RootElement.GetArrayLength());
//
//                 foreach (var element in document.RootElement.EnumerateArray())
//                 {
//                     var value = element.GetString();
//                     if (string.IsNullOrEmpty(value))
//                     {
//                         continue;
//                     }
//
//                     builder.Add(value);
//                 }
//
//                 return builder.ToImmutable();
//             });
//
//             return new ValueTask<ImmutableArray<string>>(addresses);
//         }
//
//         public virtual ValueTask<ImmutableArray<string>> GetRequirementsAsync(TApplication application, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (string.IsNullOrEmpty(application.Requirements))
//             {
//                 return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
//             }
//
//             // Note: parsing the stringified requirements is an expensive operation.
//             // To mitigate that, the resulting array is stored in the memory cache.
//             var key = string.Concat("b4808a89-8969-4512-895f-a909c62a8995", "\x1e", application.Requirements);
//             var requirements = Cache.GetOrCreate(key, entry =>
//             {
//                 entry.SetPriority(CacheItemPriority.High)
//                     .SetSlidingExpiration(TimeSpan.FromMinutes(1));
//
//                 using var document = JsonDocument.Parse(application.Requirements);
//                 var builder = ImmutableArray.CreateBuilder<string>(document.RootElement.GetArrayLength());
//
//                 foreach (var element in document.RootElement.EnumerateArray())
//                 {
//                     var value = element.GetString();
//                     if (string.IsNullOrEmpty(value))
//                     {
//                         continue;
//                     }
//
//                     builder.Add(value);
//                 }
//
//                 return builder.ToImmutable();
//             });
//
//             return new ValueTask<ImmutableArray<string>>(requirements);
//         }
//
//         public virtual ValueTask<TApplication> InstantiateAsync(CancellationToken cancellationToken = default)
//         {
//             try
//             {
//                 return new ValueTask<TApplication>(Activator.CreateInstance<TApplication>());
//             }
//
//             catch (MemberAccessException exception)
//             {
//                 return new ValueTask<TApplication>(Task.FromException<TApplication>(
//                     new InvalidOperationException(SR.GetResourceString(SR.ID0240), exception)));
//             }
//         }
//
//         public virtual IAsyncEnumerable<TApplication> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default)
//         {
//             var query = Applications.AsQueryable().OrderBy(application => application.Id!);
//
//             if (offset.HasValue)
//             {
//                 query = query.Skip(offset.Value) as IOrderedQueryable<TApplication>;
//             }
//
//             if (count.HasValue)
//             {
//                 query = query.Take(count.Value) as IOrderedQueryable<TApplication>;
//             }
//
//             return query.ToAsyncEnumerable();
//         }
//
//         public virtual  IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<TApplication>, TState, IQueryable<TResult>> query, TState state,
//             CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(query, nameof(query));
//             // ValueTask.FromCanceled(cancellationToken);
//             return query(Applications, state).ToAsyncEnumerable();
//         }
//
//         public virtual ValueTask SetClientIdAsync(TApplication application, string? identifier, CancellationToken cancellationToken = default)
//         {
//             Check.NotNull(application, nameof(application));
//
//             application.ClientId = identifier;
//
//             return default;
//         }
//
//         public virtual ValueTask SetClientSecretAsync(TApplication application, string? secret, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             application.ClientSecret = secret;
//
//             return default;
//         }
//
//         public virtual ValueTask SetClientTypeAsync(TApplication application, string? type, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             application.Type = type;
//
//             return default;
//         }
//
//         public virtual ValueTask SetConsentTypeAsync(TApplication application, string? type, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             application.ConsentType = type;
//
//             return default;
//         }
//
//         public virtual ValueTask SetDisplayNameAsync(TApplication application, string? name, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             application.DisplayName = name;
//
//             return default;
//         }
//
//         public virtual  ValueTask SetDisplayNamesAsync(TApplication application, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (names is null || names.IsEmpty)
//             {
//                 application.DisplayNames = null;
//
//                 return default;
//             }
//
//             using var stream = new MemoryStream();
//             using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
//             {
//                 Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
//                 Indented = false
//             });
//
//             writer.WriteStartObject();
//
//             foreach (var pair in names)
//             {
//                 writer.WritePropertyName(pair.Key.Name);
//                 writer.WriteStringValue(pair.Value);
//             }
//
//             writer.WriteEndObject();
//             writer.Flush();
//
//             application.DisplayNames = Encoding.UTF8.GetString(stream.ToArray());
//
//             return default;
//         }
//
//         public virtual ValueTask SetPermissionsAsync(TApplication application, ImmutableArray<string> permissions, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (permissions.IsDefaultOrEmpty)
//             {
//                 application.Permissions = null;
//
//                 return default;
//             }
//
//             using var stream = new MemoryStream();
//             using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
//             {
//                 Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
//                 Indented = false
//             });
//
//             writer.WriteStartArray();
//
//             foreach (var permission in permissions)
//             {
//                 writer.WriteStringValue(permission);
//             }
//
//             writer.WriteEndArray();
//             writer.Flush();
//
//             application.Permissions = Encoding.UTF8.GetString(stream.ToArray());
//
//             return default;
//         }
//
//         public virtual ValueTask SetPostLogoutRedirectUrisAsync(TApplication application, ImmutableArray<string> addresses, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (addresses.IsDefaultOrEmpty)
//             {
//                 application.PostLogoutRedirectUris = null;
//
//                 return default;
//             }
//
//             using var stream = new MemoryStream();
//             using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
//             {
//                 Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
//                 Indented = false
//             });
//
//             writer.WriteStartArray();
//
//             foreach (var address in addresses)
//             {
//                 writer.WriteStringValue(address);
//             }
//
//             writer.WriteEndArray();
//             writer.Flush();
//
//             application.PostLogoutRedirectUris = Encoding.UTF8.GetString(stream.ToArray());
//
//             return default;
//         }
//
//         public virtual ValueTask SetPropertiesAsync(TApplication application, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (properties is null || properties.IsEmpty)
//             {
//                 application.Properties = null;
//
//                 return default;
//             }
//
//             using var stream = new MemoryStream();
//             using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
//             {
//                 Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
//                 Indented = false
//             });
//
//             writer.WriteStartObject();
//
//             foreach (var property in properties)
//             {
//                 writer.WritePropertyName(property.Key);
//                 property.Value.WriteTo(writer);
//             }
//
//             writer.WriteEndObject();
//             writer.Flush();
//
//             application.Properties = Encoding.UTF8.GetString(stream.ToArray());
//
//             return default;
//         }
//
//         public virtual ValueTask SetRedirectUrisAsync(TApplication application, ImmutableArray<string> addresses, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (addresses.IsDefaultOrEmpty)
//             {
//                 application.RedirectUris = null;
//
//                 return default;
//             }
//
//             using var stream = new MemoryStream();
//             using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
//             {
//                 Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
//                 Indented = false
//             });
//
//             writer.WriteStartArray();
//
//             foreach (var address in addresses)
//             {
//                 writer.WriteStringValue(address);
//             }
//
//             writer.WriteEndArray();
//             writer.Flush();
//
//             application.RedirectUris = Encoding.UTF8.GetString(stream.ToArray());
//
//             return default;
//         }
//
//         public virtual ValueTask SetRequirementsAsync(TApplication application, ImmutableArray<string> requirements, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             if (requirements.IsDefaultOrEmpty)
//             {
//                 application.Requirements = null;
//
//                 return default;
//             }
//
//             using var stream = new MemoryStream();
//             using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
//             {
//                 Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
//                 Indented = false
//             });
//
//             writer.WriteStartArray();
//
//             foreach (var requirement in requirements)
//             {
//                 writer.WriteStringValue(requirement);
//             }
//
//             writer.WriteEndArray();
//             writer.Flush();
//
//             application.Requirements = Encoding.UTF8.GetString(stream.ToArray());
//
//             return default;
//         }
//
//         public virtual async ValueTask UpdateAsync(TApplication application, CancellationToken cancellationToken)
//         {
//             Check.NotNull(application, nameof(application));
//
//             var findApplication = await Applications.GetAsync(application.Id, cancellationToken);
//
//             findApplication.ConcurrencyToken = Guid.NewGuid().ToString();
//
//             try
//             {
//                 await Applications.UpdateAsync(findApplication, AutoSaveChanges, cancellationToken);
//             }
//             catch (DBConcurrencyException ex)
//             {
//                 throw new OpenIddictExceptions.ConcurrencyException(SR.GetResourceString(SR.ID0239), ex);
//             }
//         }
//
//         public virtual TKey? ConvertIdentifierFromString(string? identifier)
//         {
//             if (string.IsNullOrEmpty(identifier))
//             {
//                 return default;
//             }
//
//             return (TKey) TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(identifier);
//         }
//
//         public virtual string? ConvertIdentifierToString(TKey? identifier)
//         {
//             if (Equals(identifier, default(TKey)))
//             {
//                 return null;
//             }
//
//             return TypeDescriptor.GetConverter(typeof(TKey)).ConvertToInvariantString(identifier);
//         }
//     }
// }