using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ReinaFIEC.Data
{
    /// <summary>
    /// Interfaz genérica para operaciones de repositorio
    /// Implementa el patrón Repository para abstracción de acceso a datos
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        T ObtenerPorId(int id);

        /// <summary>
        /// Obtiene todas las entidades
        /// </summary>
        IEnumerable<T> ObtenerTodos();

        /// <summary>
        /// Busca entidades que cumplan con una condición
        /// </summary>
        IEnumerable<T> Buscar(Expression<Func<T, bool>> predicado);

        /// <summary>
        /// Obtiene una única entidad que cumple una condición
        /// </summary>
        T ObtenerUno(Expression<Func<T, bool>> predicado);

        /// <summary>
        /// Agrega una nueva entidad
        /// </summary>
        void Agregar(T entidad);

        /// <summary>
        /// Actualiza una entidad existente
        /// </summary>
        void Actualizar(T entidad);

        /// <summary>
        /// Elimina una entidad por su ID
        /// </summary>
        void Eliminar(int id);

        /// <summary>
        /// Elimina una entidad
        /// </summary>
        void Eliminar(T entidad);

        /// <summary>
        /// Cuenta el número de entidades
        /// </summary>
        int Contar();

        /// <summary>
        /// Cuenta el número de entidades que cumplen una condición
        /// </summary>
        int Contar(Expression<Func<T, bool>> predicado);

        /// <summary>
        /// Verifica si existe alguna entidad que cumpla la condición
        /// </summary>
        bool Existe(Expression<Func<T, bool>> predicado);
    }
}
