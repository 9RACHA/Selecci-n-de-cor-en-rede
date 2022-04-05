using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;


namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        //Las variables son compartidas en el servidor y cliente y se sincronizan
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        public NetworkVariable<Color> colorJugador = new NetworkVariable<Color>();

        //network variable de color sino de material
        public static List<Color> coloresDisponibles = new List<Color>(); //colores disponibles

        Renderer render;

        //CLASE
         void Start() {

            //On Evento
            Position.OnValueChanged += OnPositionChange;    //Solo si cambia la posicion de position actualiza el valor
            render = GetComponent<Renderer>();
            colorJugador.OnValueChanged += OnColorChange;   //Al cambiar el valor se actualiza
            

            if (IsServer && IsOwner) {  //Hace que no se repita al inciar el start

            coloresDisponibles.Add(Color.blue);
            coloresDisponibles.Add(Color.gray);
            coloresDisponibles.Add(Color.green);
            coloresDisponibles.Add(Color.black);
            coloresDisponibles.Add(Color.cyan);
            coloresDisponibles.Add(Color.magenta);
            coloresDisponibles.Add(Color.yellow);
            coloresDisponibles.Add(Color.red);
            coloresDisponibles.Add(Color.white);

            Debug.Log(coloresDisponibles.Count); //9 Colores disponibles
            }

            if (IsOwner)
            {
                SubmitColorRequestServerRpc(true);
            }
            Debug.Log("Start");
        }

        public void Colorea(){
            render.material.SetColor("_Color", coloresDisponibles[Random.Range(0,coloresDisponibles.Count)]);
        }

        //Solo actualiza cuando hay un cambio de valor y no cada frame cuando estaba en el Update
        public void OnPositionChange(Vector3 previousValue, Vector3 newValue){
            transform.position = Position.Value;
        }

        //MODIFICAR ESTE METODO
        public void OnColorChange(Color colorAntiguo, Color nuevoColor){
            render.material.color = colorJugador.Value;
        }
        

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Move();
            }
        /*    else if (IsClient)
            {
                Colorea();
            }
            */
        }

        //MODIFICAR ESTE METODO
        /*
        public void Colorea()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
            }
            else
            {
                SubmitPositionRequestServerRpc();
            }
             
             //   SubmitColorRequestServerRpc();   //Esta linea hace lo mismo sin el if y el else
        }
        */

        public void Move()
        {
            //SubmitPositionRequestServerRpc();
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
            }
            else
            {
                SubmitPositionRequestServerRpc();
            } 
        }

        [ServerRpc] //SIEMPRE TIPO VOID por tanto no devuelve nada
        public void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Position.Value = GetRandomPositionOnPlane(); //La posicion de aquien llamo el server rpc
        }

         [ServerRpc] //SIEMPRE TIPO VOID
        public void SubmitColorRequestServerRpc(bool primeravez = false, ServerRpcParams rpcParams = default)
        {
            //CREA DOS VARIABLES el color asignado 
            Color oldColor = colorJugador.Value;    // color antiguo del jugador
            Color newColor = coloresDisponibles[Random.Range(0,coloresDisponibles.Count)];  // asignan un nuevo color generando un aleatorio
            coloresDisponibles.Remove(newColor);  //Borra el nuevo color
            if (!primeravez)    //Si no es el primer color
            {
                coloresDisponibles.Add(oldColor); //Añade el color antiguo para evitar quedarse sin colores
            }
            colorJugador.Value = newColor;
            Debug.Log(coloresDisponibles); //
            //render.material.SetColor("_Color", listaColores[Random.Range(0,listaColores.Count)]);
            //La posicion de a quien llamo el server rpc
        }


        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        /*
        static Material GetRandomColorOnPlane(){
            return new Material(Random.Range(coloresDisponibles.Count));
        }
        */


        void Update()
        {
           render.material.SetColor("Color", colorJugador.Value); //OnValueChanged para actualizar el valor
           // render.material.color = colorJugador.Value;
           //transform.position = Posicion.Value; 
        }
    }
}
