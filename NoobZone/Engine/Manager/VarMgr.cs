using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoobZone
{
    /// <summary>
    /// Gestionnaire de Variables d'environnement
    /// </summary>
    class VarMgr
    {
        struct Variable
        {
            public Type type;
            public int ival;
            public float fval;
            public bool bval;
        }

        static Dictionary<string, Variable> m_varDataBase = new Dictionary<string, Variable>();

        /// <summary>
        /// Set/Add an int value in the variable manager
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <param name="value">Value of the variable</param>
        public static void Seti(string name, int value)
        {
            Variable var = new Variable();
            var.type = typeof(int);
            var.ival = value;
            if(!m_varDataBase.ContainsKey(name))
                m_varDataBase.Add(name, var);
            else
                m_varDataBase[name] = var;
        }
        /// <summary>
        /// Set/Add a float value in the variable manager
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <param name="value">Value of the variable</param>
        public static void Setf(string name, float value)
        {
            Variable var = new Variable();
            var.type = typeof(float);
            var.fval = value;
            if(!m_varDataBase.ContainsKey(name))
                m_varDataBase.Add(name, var);
            else
                m_varDataBase[name] = var;
        }
        /// <summary>
        /// Set/Add a bool value in the variable manager
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <param name="value">Value of the variable</param>
        public static void Setb(string name, bool value)
        {
            Variable var = new Variable();
            var.type = typeof(bool);
            var.bval = value;
            if (!m_varDataBase.ContainsKey(name))
                m_varDataBase.Add(name, var);
            else
                m_varDataBase[name] = var;
        }
        /// <summary>
        /// Get an int value from the variable manager
        /// </summary>
        /// <param name="name">Name of the variable</param>
        public static int Geti(string name)
        {
            return m_varDataBase[name].ival;
        }
        /// <summary>
        /// Get a float value from the variable manager
        /// </summary>
        /// <param name="name">Name of the variable</param>
        public static float Getf(string name)
        {
            return m_varDataBase[name].fval;
        }
        /// <summary>
        /// Get a bool value from the variable manager
        /// </summary>
        /// <param name="name">Name of the variable</param>
        public static bool Getb(string name)
        {
            return m_varDataBase[name].bval;
        }
        /// <summary>
        /// Clear the Variable Manager Database
        /// </summary>
        public static bool Destroy(string name)
        {
            return m_varDataBase.Remove(name);
        }
        public static void Clear()
        {
            m_varDataBase.Clear();
        }
    }
}
