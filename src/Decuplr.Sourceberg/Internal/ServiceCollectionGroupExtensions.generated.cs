
using Microsoft.Extensions.DependencyInjection;


namespace Decuplr {
	
	internal static partial class ServiceCollectionGroupExtensions {
		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1> (this IServiceCollection services) 
			where T0 : class, T1
			where T1 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2> (this IServiceCollection services) 
			where T0 : class, T1, T2
			where T1 : class
			where T2 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3
			where T1 : class
			where T2 : class
			where T3 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5, T6> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T6>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5, T6, T7> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T6>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T7>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T6>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T7>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T8>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T6>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T7>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T8>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T9>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T6>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T7>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T8>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T9>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T10>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T6>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T7>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T8>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T9>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T10>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T11>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T6>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T7>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T8>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T9>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T10>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T11>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T12>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/>, <typeparamref name="T13"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		/// <typeparam name="T12">The 12 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
			where T13 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T6>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T7>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T8>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T9>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T10>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T11>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T12>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T13>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/>, <typeparamref name="T13"/>, <typeparamref name="T14"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		/// <typeparam name="T12">The 12 type of the service to add.</typeparam>
		/// <typeparam name="T13">The 13 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
			where T13 : class
			where T14 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T6>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T7>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T8>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T9>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T10>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T11>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T12>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T13>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T14>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of singleton service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/>, <typeparamref name="T13"/>, <typeparamref name="T14"/>, <typeparamref name="T15"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		/// <typeparam name="T12">The 12 type of the service to add.</typeparam>
		/// <typeparam name="T13">The 13 type of the service to add.</typeparam>
		/// <typeparam name="T14">The 14 type of the service to add.</typeparam>
		public static IServiceCollection AddSingletonGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
			where T13 : class
			where T14 : class
			where T15 : class
		{
			services.AddSingleton<T0>();
			services.AddSingleton<T1>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T2>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T3>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T4>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T5>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T6>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T7>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T8>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T9>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T10>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T11>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T12>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T13>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T14>(x => x.GetRequiredService<T0>());
			services.AddSingleton<T15>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1> (this IServiceCollection services) 
			where T0 : class, T1
			where T1 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2> (this IServiceCollection services) 
			where T0 : class, T1, T2
			where T1 : class
			where T2 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3
			where T1 : class
			where T2 : class
			where T3 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5, T6> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			services.AddScoped<T6>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5, T6, T7> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			services.AddScoped<T6>(x => x.GetRequiredService<T0>());
			services.AddScoped<T7>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			services.AddScoped<T6>(x => x.GetRequiredService<T0>());
			services.AddScoped<T7>(x => x.GetRequiredService<T0>());
			services.AddScoped<T8>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			services.AddScoped<T6>(x => x.GetRequiredService<T0>());
			services.AddScoped<T7>(x => x.GetRequiredService<T0>());
			services.AddScoped<T8>(x => x.GetRequiredService<T0>());
			services.AddScoped<T9>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			services.AddScoped<T6>(x => x.GetRequiredService<T0>());
			services.AddScoped<T7>(x => x.GetRequiredService<T0>());
			services.AddScoped<T8>(x => x.GetRequiredService<T0>());
			services.AddScoped<T9>(x => x.GetRequiredService<T0>());
			services.AddScoped<T10>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			services.AddScoped<T6>(x => x.GetRequiredService<T0>());
			services.AddScoped<T7>(x => x.GetRequiredService<T0>());
			services.AddScoped<T8>(x => x.GetRequiredService<T0>());
			services.AddScoped<T9>(x => x.GetRequiredService<T0>());
			services.AddScoped<T10>(x => x.GetRequiredService<T0>());
			services.AddScoped<T11>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			services.AddScoped<T6>(x => x.GetRequiredService<T0>());
			services.AddScoped<T7>(x => x.GetRequiredService<T0>());
			services.AddScoped<T8>(x => x.GetRequiredService<T0>());
			services.AddScoped<T9>(x => x.GetRequiredService<T0>());
			services.AddScoped<T10>(x => x.GetRequiredService<T0>());
			services.AddScoped<T11>(x => x.GetRequiredService<T0>());
			services.AddScoped<T12>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/>, <typeparamref name="T13"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		/// <typeparam name="T12">The 12 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
			where T13 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			services.AddScoped<T6>(x => x.GetRequiredService<T0>());
			services.AddScoped<T7>(x => x.GetRequiredService<T0>());
			services.AddScoped<T8>(x => x.GetRequiredService<T0>());
			services.AddScoped<T9>(x => x.GetRequiredService<T0>());
			services.AddScoped<T10>(x => x.GetRequiredService<T0>());
			services.AddScoped<T11>(x => x.GetRequiredService<T0>());
			services.AddScoped<T12>(x => x.GetRequiredService<T0>());
			services.AddScoped<T13>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/>, <typeparamref name="T13"/>, <typeparamref name="T14"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		/// <typeparam name="T12">The 12 type of the service to add.</typeparam>
		/// <typeparam name="T13">The 13 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
			where T13 : class
			where T14 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			services.AddScoped<T6>(x => x.GetRequiredService<T0>());
			services.AddScoped<T7>(x => x.GetRequiredService<T0>());
			services.AddScoped<T8>(x => x.GetRequiredService<T0>());
			services.AddScoped<T9>(x => x.GetRequiredService<T0>());
			services.AddScoped<T10>(x => x.GetRequiredService<T0>());
			services.AddScoped<T11>(x => x.GetRequiredService<T0>());
			services.AddScoped<T12>(x => x.GetRequiredService<T0>());
			services.AddScoped<T13>(x => x.GetRequiredService<T0>());
			services.AddScoped<T14>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of scoped service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/>, <typeparamref name="T13"/>, <typeparamref name="T14"/>, <typeparamref name="T15"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		/// <typeparam name="T12">The 12 type of the service to add.</typeparam>
		/// <typeparam name="T13">The 13 type of the service to add.</typeparam>
		/// <typeparam name="T14">The 14 type of the service to add.</typeparam>
		public static IServiceCollection AddScopedGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
			where T13 : class
			where T14 : class
			where T15 : class
		{
			services.AddScoped<T0>();
			services.AddScoped<T1>(x => x.GetRequiredService<T0>());
			services.AddScoped<T2>(x => x.GetRequiredService<T0>());
			services.AddScoped<T3>(x => x.GetRequiredService<T0>());
			services.AddScoped<T4>(x => x.GetRequiredService<T0>());
			services.AddScoped<T5>(x => x.GetRequiredService<T0>());
			services.AddScoped<T6>(x => x.GetRequiredService<T0>());
			services.AddScoped<T7>(x => x.GetRequiredService<T0>());
			services.AddScoped<T8>(x => x.GetRequiredService<T0>());
			services.AddScoped<T9>(x => x.GetRequiredService<T0>());
			services.AddScoped<T10>(x => x.GetRequiredService<T0>());
			services.AddScoped<T11>(x => x.GetRequiredService<T0>());
			services.AddScoped<T12>(x => x.GetRequiredService<T0>());
			services.AddScoped<T13>(x => x.GetRequiredService<T0>());
			services.AddScoped<T14>(x => x.GetRequiredService<T0>());
			services.AddScoped<T15>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1> (this IServiceCollection services) 
			where T0 : class, T1
			where T1 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2> (this IServiceCollection services) 
			where T0 : class, T1, T2
			where T1 : class
			where T2 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3
			where T1 : class
			where T2 : class
			where T3 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5, T6> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			services.AddTransient<T6>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5, T6, T7> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			services.AddTransient<T6>(x => x.GetRequiredService<T0>());
			services.AddTransient<T7>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			services.AddTransient<T6>(x => x.GetRequiredService<T0>());
			services.AddTransient<T7>(x => x.GetRequiredService<T0>());
			services.AddTransient<T8>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			services.AddTransient<T6>(x => x.GetRequiredService<T0>());
			services.AddTransient<T7>(x => x.GetRequiredService<T0>());
			services.AddTransient<T8>(x => x.GetRequiredService<T0>());
			services.AddTransient<T9>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			services.AddTransient<T6>(x => x.GetRequiredService<T0>());
			services.AddTransient<T7>(x => x.GetRequiredService<T0>());
			services.AddTransient<T8>(x => x.GetRequiredService<T0>());
			services.AddTransient<T9>(x => x.GetRequiredService<T0>());
			services.AddTransient<T10>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			services.AddTransient<T6>(x => x.GetRequiredService<T0>());
			services.AddTransient<T7>(x => x.GetRequiredService<T0>());
			services.AddTransient<T8>(x => x.GetRequiredService<T0>());
			services.AddTransient<T9>(x => x.GetRequiredService<T0>());
			services.AddTransient<T10>(x => x.GetRequiredService<T0>());
			services.AddTransient<T11>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			services.AddTransient<T6>(x => x.GetRequiredService<T0>());
			services.AddTransient<T7>(x => x.GetRequiredService<T0>());
			services.AddTransient<T8>(x => x.GetRequiredService<T0>());
			services.AddTransient<T9>(x => x.GetRequiredService<T0>());
			services.AddTransient<T10>(x => x.GetRequiredService<T0>());
			services.AddTransient<T11>(x => x.GetRequiredService<T0>());
			services.AddTransient<T12>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/>, <typeparamref name="T13"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		/// <typeparam name="T12">The 12 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
			where T13 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			services.AddTransient<T6>(x => x.GetRequiredService<T0>());
			services.AddTransient<T7>(x => x.GetRequiredService<T0>());
			services.AddTransient<T8>(x => x.GetRequiredService<T0>());
			services.AddTransient<T9>(x => x.GetRequiredService<T0>());
			services.AddTransient<T10>(x => x.GetRequiredService<T0>());
			services.AddTransient<T11>(x => x.GetRequiredService<T0>());
			services.AddTransient<T12>(x => x.GetRequiredService<T0>());
			services.AddTransient<T13>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/>, <typeparamref name="T13"/>, <typeparamref name="T14"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		/// <typeparam name="T12">The 12 type of the service to add.</typeparam>
		/// <typeparam name="T13">The 13 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
			where T13 : class
			where T14 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			services.AddTransient<T6>(x => x.GetRequiredService<T0>());
			services.AddTransient<T7>(x => x.GetRequiredService<T0>());
			services.AddTransient<T8>(x => x.GetRequiredService<T0>());
			services.AddTransient<T9>(x => x.GetRequiredService<T0>());
			services.AddTransient<T10>(x => x.GetRequiredService<T0>());
			services.AddTransient<T11>(x => x.GetRequiredService<T0>());
			services.AddTransient<T12>(x => x.GetRequiredService<T0>());
			services.AddTransient<T13>(x => x.GetRequiredService<T0>());
			services.AddTransient<T14>(x => x.GetRequiredService<T0>());
			return services;
		}

		
		/// <summary>
		///  Adds a set of transient service of the type specified in <typeparamref name="T1"/>, <typeparamref name="T2"/>, <typeparamref name="T3"/>, <typeparamref name="T4"/>, <typeparamref name="T5"/>, <typeparamref name="T6"/>, <typeparamref name="T7"/>, <typeparamref name="T8"/>, <typeparamref name="T9"/>, <typeparamref name="T10"/>, <typeparamref name="T11"/>, <typeparamref name="T12"/>, <typeparamref name="T13"/>, <typeparamref name="T14"/>, <typeparamref name="T15"/> with an implementation
        ///  type specified in <typeparamref name="T0"/> to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
		/// </summary>
		/// <typeparam name="T0">The type of the implementation to use.</typeparam>
		/// <typeparam name="T1">The 1 type of the service to add.</typeparam>
		/// <typeparam name="T2">The 2 type of the service to add.</typeparam>
		/// <typeparam name="T3">The 3 type of the service to add.</typeparam>
		/// <typeparam name="T4">The 4 type of the service to add.</typeparam>
		/// <typeparam name="T5">The 5 type of the service to add.</typeparam>
		/// <typeparam name="T6">The 6 type of the service to add.</typeparam>
		/// <typeparam name="T7">The 7 type of the service to add.</typeparam>
		/// <typeparam name="T8">The 8 type of the service to add.</typeparam>
		/// <typeparam name="T9">The 9 type of the service to add.</typeparam>
		/// <typeparam name="T10">The 10 type of the service to add.</typeparam>
		/// <typeparam name="T11">The 11 type of the service to add.</typeparam>
		/// <typeparam name="T12">The 12 type of the service to add.</typeparam>
		/// <typeparam name="T13">The 13 type of the service to add.</typeparam>
		/// <typeparam name="T14">The 14 type of the service to add.</typeparam>
		public static IServiceCollection AddTransientGroup<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> (this IServiceCollection services) 
			where T0 : class, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
			where T5 : class
			where T6 : class
			where T7 : class
			where T8 : class
			where T9 : class
			where T10 : class
			where T11 : class
			where T12 : class
			where T13 : class
			where T14 : class
			where T15 : class
		{
			services.AddTransient<T0>();
			services.AddTransient<T1>(x => x.GetRequiredService<T0>());
			services.AddTransient<T2>(x => x.GetRequiredService<T0>());
			services.AddTransient<T3>(x => x.GetRequiredService<T0>());
			services.AddTransient<T4>(x => x.GetRequiredService<T0>());
			services.AddTransient<T5>(x => x.GetRequiredService<T0>());
			services.AddTransient<T6>(x => x.GetRequiredService<T0>());
			services.AddTransient<T7>(x => x.GetRequiredService<T0>());
			services.AddTransient<T8>(x => x.GetRequiredService<T0>());
			services.AddTransient<T9>(x => x.GetRequiredService<T0>());
			services.AddTransient<T10>(x => x.GetRequiredService<T0>());
			services.AddTransient<T11>(x => x.GetRequiredService<T0>());
			services.AddTransient<T12>(x => x.GetRequiredService<T0>());
			services.AddTransient<T13>(x => x.GetRequiredService<T0>());
			services.AddTransient<T14>(x => x.GetRequiredService<T0>());
			services.AddTransient<T15>(x => x.GetRequiredService<T0>());
			return services;
		}

	}
}