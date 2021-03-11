using System;
using System.Collections.Generic;

namespace ControleEstoque.Web.Models
{
    public class EntradaSaidaProdutoViewModel
    {
        public DateTime Data { get; set; }

        // - ABAIXO: Temos um dicionário com 2 int - o 1º "ID" e o 2º a "Quantidade" (PRODUTO)...
        public Dictionary<int, int> Produtos { get; set; }
    }
}