using System;

public class Heap<Node> where Node : IHeapItem<Node>
{
    //Array con los nodos
    Node[] items;
    //Tamano del array
    int currentItemCount;
    //COnstructor del array
    public Heap(int maxHeapSize)
    {
        items = new Node[maxHeapSize];
    }
    //Propiedad para conseguir el tamano del array
    public int Count
    {
        get 
        {
            return currentItemCount;
        }
    }
    //Funcion para limpiar el array
    public void Clear()
    {
        currentItemCount = 0;
    }
    //Funcion para anadir nodos en el heap
    public void Add(Node item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }
    //Funcion para eliminar el primer nodo
    public Node RemoveFirstItem()
    {
        Node firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }
    //Se actualiza el nodo especificado
    public void UpdateItem(Node item)
    {
        SortUp(item);
    }
    //Funcion para comprobar si el nodo esta en el array
    public bool Contains(Node item) 
    {
        if (item.HeapIndex < currentItemCount)
        {
            return Equals(items[item.HeapIndex], item);
        }
        else
        {
            return false;
        }
    }
    //Funcion para ordenar el nodo de menor a mayor
    void SortDown(Node item)
    {
        while(true)
        {
            //Se obtiene le hijo de la izquierda multiplicando el indice por los dos hijos del padre mas 1 por ser el hijo de la izquierda
            int childIndexLeft = item.HeapIndex * 2 + 1;
            //Se obtiene le hijo de la derecha multiplicando el indice por los dos hijos del padre mas 2 por ser el hijo de la derecha
            int childIndexRight = item.HeapIndex * 2 + 2;
            //Variable para definir el nodo  que se debe de cambiar
            int swapIndex = 0;
            //Si el hijo de la izquierda esta dentro del array de nodos
            if (childIndexLeft < currentItemCount)
            {
                //Se anade como indice a intercambiar
                swapIndex = childIndexLeft;
                //Si el hijo de la izquierda esta dentro del array de nodos
                if (childIndexRight < currentItemCount)
                {
                    //Si el nodo que se encuentra como hijo de la izquierda es menor que el hijo de la derecha
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        //Se anade como indice a intercambiar
                        swapIndex = childIndexRight;
                    }
                }
                //Se compara el nodo recibido con el nodo a intercambiar
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    //Se intercambian entre si si el nodo actual es menor que el nodo a intercambiar
                    SwapItems(item, items[swapIndex]);
                }
                //Si no salimos del bucle
                else
                {
                    break;
                }
            }
            //Si no salimos del bucle
            else
            {
                break;
            }
        }
    }
    //Se ordena en orden para encontrar el padre del nodo
    void SortUp(Node item)
    {
        //Para obtener el indice del padre se redondea el resultado del indice del nodo se le resta 1 para encontrar a su padre y se divide por los dos hijos
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            //Obtenemos el padre
            Node parentItem = items[parentIndex];
            //Comparamos si el nodo nuevo es mayor que el padre
            if (item.CompareTo(parentItem) > 0)
            {
                //Los intercambiamos 
                SwapItems(item, parentItem);
            }
            else 
            {
                //Si no, es porque ya no hay mas nodos por mirar 
                break;
            }
            //Se actualiza el indice padre
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }
    //Funcion para intercambiar los nodos
    void SwapItems(Node itemA, Node itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}
//Interfaz donde definimos que se puede intercambiar el indice del heap
public interface IHeapItem<Node> : IComparable<Node>
{
    int HeapIndex 
    { 
        get; 
        set; 
    }
}
