using Microsoft.Azure.Cosmos;
using MvcCoreCosmosDb.Models;

namespace MvcCoreCosmosDb.Services
{
    public class ServiceCosmosDb
    {
        //DENTRO DE COSMO SE TRABAJA CON LOS CONTAINERS
        // QUE ES DONDE ESTAN LOS ITEMS, EN NUESTRO EJEMPLO:
        private CosmosClient cosmosClient;
        private Container containerCosmos;
        public ServiceCosmosDb(CosmosClient cosmosClient, Container containerCosmos)
        {
            this.cosmosClient = cosmosClient;
            this.containerCosmos = containerCosmos;
        }
        //VAMOS A CREAR UN METODO PARA LA BASE DE DATOS
        // Y EL CONTENEDOR DE COSMOS DB EN CELESTE
        public async Task CreateDatabaseAsync()
        {
            await this.cosmosClient.CreateDatabaseIfNotExistsAsync
                ("vehiculoscosmos");
            //CREAMOS UN CONTENEDOR PARA LA BBDD
            ContainerProperties properties =
                new ContainerProperties("containercoches","/id");
            //Agregamos el contenedor a la bbdd
            await this.cosmosClient.GetDatabase("vehiculoscosmos")
                .CreateContainerIfNotExistsAsync(properties);
        }
        public async Task CreateCocheAsync(Coche car)
        {
            //CUANDO INSERTAMOS DEBEMOS INDICAR DE FORMA EXPLICITA EL DATO
            // PARA EL PARTITION KEY
            await this.containerCosmos
                .CreateItemAsync<Coche>(car, new PartitionKey(car.Id));
        }
        //Recuperamo todos los datos
        public async Task<List<Coche>> GetCochesAsync()
        {
            //NOSQL NUNA SABE LOS REGISTROS TOTALES, NO FUNCIONA ASI,
            // DEBEMOS RECORRER CON UN WHILE PARA EXTRAER LOS REGISTROS
            var query = this.containerCosmos.GetItemQueryIterator<Coche>();
            List<Coche> coches = new List<Coche>();
            while (query.HasMoreResults)
            {
                var resultado = await query.ReadNextAsync();
                //POR CADA RESULTADO AGREGAMOS A LA COLECCCION AUN  QUE SOLO SEA
                // COCHE A COCHE DEVUELVE COMO SI FUERAN VARIOS COCHES
                coches.AddRange(resultado);
            }
            return coches;
        }
        //BUSCAR COCHES. PARA BUSCAR UN COCHE SE HACE POR SU ID Y POR SU PARTITION KEY
        public async Task<Coche> FindCocheAsync(string id)
        {
            ItemResponse<Coche> response = await this.containerCosmos.ReadItemAsync<Coche>
                (id.ToString(), new PartitionKey(id));
            return response.Resource;
        }
        public async Task DeleteCocheAsync(string id)
        {
            await this.containerCosmos.DeleteItemAsync<Coche>
                (id.ToString(), new PartitionKey(id));
        }
        public async Task UpdateCocheAsync(Coche coche)
        {
            await this.containerCosmos.UpsertItemAsync<Coche>
                (coche, new PartitionKey(coche.Id));
        }
    }
        
}
