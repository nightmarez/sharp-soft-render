using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Renderer
{
    /// <summary>
    /// Фабрика загрузчиков 3D моделей.
    /// </summary>
    public sealed class LoadersFactory
    {
        /// <summary>
        /// Кэш загрузчиков.
        /// </summary>
        private readonly Dictionary<string, ILoader> _loaders = new Dictionary<string, ILoader>();

        /// <summary>
        /// Возвращает загрузчик для заданного файла.
        /// </summary>
        public ILoader GetLoader(string fileName)
        {
            string ext = (Path.GetExtension(fileName) ?? string.Empty).ToLowerInvariant();

            if (_loaders.ContainsKey(ext))
                return _loaders[ext];

            Type loaderInterface = typeof (ILoader);

            Type loaderClass = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass)
                .FirstOrDefault(t => t.GetInterfaces().Any(i => i == loaderInterface));

            return loaderClass == null ? null : _loaders[ext] = (ILoader)Activator.CreateInstance(loaderClass);
        }
    }
}
