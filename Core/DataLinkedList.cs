using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Collections.ObjectModel;

namespace Core
{

	/// <summary>Represents a doubly linked list.</summary>
	/// <typeparam name="T">Specifies the element type of the linked list.</typeparam>
	[Serializable]
	[ComVisible(false)]
	[DebuggerDisplay("Count = {Count}")]
	public class DataLinkedList<T> : ObservableCollection<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback where T : IDataGoo
	{
        #region Enumerator

        /// <summary>Enumerates the elements of a <see cref="T:Core.CustomLinkedList<T>" />.</summary>
        /// <typeparam name="T" />
        [Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator, ISerializable, IDeserializationCallback
		{
			private DataLinkedList<T> enumlist;

			private DataLinkedListNode<T> enumhead;

			private int version;

			private T current;

			private Guid currentid;

			private int index;

			private SerializationInfo siInfo;

			/// <summary>Gets the element at the current position of the enumerator.</summary>
			/// <returns>The element in the <see cref="T:Core.CustomLinkedList<T>" /> at the current position of the enumerator.</returns>
			
			public T Current
			{				
				get
				{
					return current;
				}
			}
			public Guid CurrentGuid
			{
				get
				{
					return currentid;
				}
			}

			/// <summary>Gets the element at the current position of the enumerator.</summary>
			/// <returns>The element in the collection at the current position of the enumerator.</returns>
			/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>

			object IEnumerator.Current
			{
				
				get
				{
					if (index == 0 || index == enumlist.Count + 1)
					{
                        throw new InvalidOperationException("The enumerator is positioned before the first element of the collection or after the last element.");
					}
					return current;
				}
			}

			internal Enumerator(DataLinkedList<T> list)
			{
				this.enumlist = list;
				version = list.version;
				enumhead = list.head;
				current = default(T);
                currentid = default(Guid);
                index = 0;
				siInfo = null;
			}

			internal Enumerator(SerializationInfo info, StreamingContext context)
			{
				siInfo = info;
				enumlist = null;
				version = 0;
				enumhead = null;
				current = default(T);
				currentid = default(Guid);
				index = 0;
			}

			/// <summary>Advances the enumerator to the next element of the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
			/// <returns>
			///   <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			
			public bool MoveNext()
			{
				if (version != enumlist.version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				if (enumhead == null)
				{
					index = enumlist.Count + 1;
					return false;
				}
				index++;
				current = enumhead.item;
                currentid = enumhead.item.ID;
                enumhead = enumhead.next;
				if (enumhead == enumlist.head)
				{
					enumhead = null;
				}
				return true;
			}

			/// <summary>Sets the enumerator to its initial position, which is before the first element in the collection. This class cannot be inherited.</summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			
			void IEnumerator.Reset()
			{
				if (version != enumlist.version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				current = default(T);
                currentid = default(Guid);
                enumhead = enumlist.head;
				index = 0;
			}

			/// <summary>Releases all resources used by the <see cref="T:Core.CustomLinkedList<T>.Enumerator" />.</summary>
			
			public void Dispose()
			{
			}

			/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and returns the data needed to serialize the <see cref="T:Core.CustomLinkedList<T>" /> instance.</summary>
			/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information required to serialize the <see cref="T:Core.CustomLinkedList<T>" /> instance.</param>
			/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> object that contains the source and destination of the serialized stream associated with the <see cref="T:Core.CustomLinkedList<T>" /> instance.</param>
			/// <exception cref="T:System.ArgumentNullException">
			///   <paramref name="info" /> is <see langword="null" />.</exception>
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("CustomLinkedList", enumlist);
				info.AddValue("Version", version);
				info.AddValue("Current", current);
                info.AddValue("CurrentID", currentid);
                info.AddValue("Index", index);
			}

			/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and raises the deserialization event when the deserialization is complete.</summary>
			/// <param name="sender">The source of the deserialization event.</param>
			/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object associated with the current <see cref="T:Core.CustomLinkedList<T>" /> instance is invalid.</exception>
			void IDeserializationCallback.OnDeserialization(object sender)
			{
				if (enumlist != null)
				{
					return;
				}
				if (siInfo == null)
				{
					throw new SerializationException("Serialization_InvalidOnDeser");
				}
				enumlist = (DataLinkedList<T>)siInfo.GetValue("CustomLinkedList", typeof(DataLinkedList<T>));
				version = siInfo.GetInt32("Version");
				current = (T)siInfo.GetValue("Current", typeof(T));
                currentid = (Guid)siInfo.GetValue("CurrentID", typeof(Guid));
                index = siInfo.GetInt32("Index");
				if (enumlist.siInfo != null)
				{
					enumlist.OnDeserialization(sender);
				}
				if (index == enumlist.Count + 1)
				{
					enumhead = null;
				}
				else
				{
					enumhead = enumlist.First;
					if (enumhead != null && index != 0)
					{
						for (int i = 0; i < index; i++)
						{
							enumhead = enumhead.next;
						}
						if (enumhead == enumlist.First)
						{
							enumhead = null;
						}
					}
				}
				siInfo = null;
			}
		}

		#endregion

		#region GetEnumerator Methods

		/// <summary>Returns an enumerator that iterates through the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <returns>An <see cref="T:Core.CustomLinkedList<T>.Enumerator" /> for the <see cref="T:Core.CustomLinkedList<T>" />.</returns>
		//DONE
		public new Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.ObjectModel.Collection`1" />.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1" /> for the <see cref="T:System.Collections.ObjectModel.Collection`1" />.</returns>
		//DONE
		public IEnumerator<T> GetCollectionEnumerator()
		{
			return base.GetEnumerator();
		}

		#endregion

		#region Serialization

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and returns the data needed to serialize the <see cref="T:Core.CustomLinkedList<T>" /> instance.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information required to serialize the <see cref="T:Core.CustomLinkedList<T>" /> instance.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> object that contains the source and destination of the serialized stream associated with the <see cref="T:Core.CustomLinkedList<T>" /> instance.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", version);
			info.AddValue("Count", count);
			if (count != 0)
			{
				T[] array = new T[Count];
				this.CopyTo(array, 0);
				info.AddValue("Data", array, typeof(T[]));
			}
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and raises the deserialization event when the deserialization is complete.</summary>
		/// <param name="sender">The source of the deserialization event.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object associated with the current <see cref="T:Core.CustomLinkedList<T>" /> instance is invalid.</exception>
		//DONE
		public virtual void OnDeserialization(object sender)
		{
			if (siInfo == null)
			{
				return;
			}
			int @int = siInfo.GetInt32("Version");
			if (siInfo.GetInt32("Count") != 0)
			{
				T[] array = (T[])siInfo.GetValue("Data", typeof(T[]));
				if (array == null)
				{
					throw new SerializationException("Serialization_MissingValues");
				}
				for (int i = 0; i < array.Length; i++)
				{
					AddLast(array[i]);
					base.Add(array[i]);
				}
			}
			else
			{
				head = null;
			}
			version = @int;
			siInfo = null;
		}

		#endregion

		#region DataMembers

		internal DataLinkedListNode<T> head;

		internal int count;

		internal int version;

		private object _syncRoot;

		private SerializationInfo siInfo;

		//private const string VersionName = "Version";

		//private const string CountName = "Count";

		//private const string ValuesName = "Data";

		#endregion

		#region Properties

		/// <summary>Gets the number of nodes actually contained in the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <returns>The number of nodes actually contained in the <see cref="T:Core.CustomLinkedList<T>" />.</returns>

		public new int Count
        {
            get
            {
				//base.Count;
				//count
				count = base.Count;
                return count;
            }
        }

        /// <summary>Gets the first node of the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
        /// <returns>The first <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> of the <see cref="T:Core.CustomLinkedList<T>" />.</returns>

        public DataLinkedListNode<T> First
		{
			
			get
			{
				return head;
			}
		}

		/// <summary>Gets the last node of the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <returns>The last <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> of the <see cref="T:Core.CustomLinkedList<T>" />.</returns>
		
		public DataLinkedListNode<T> Last
		{
			
			get
			{
				if (head != null)
				{
					return head.prev;
				}
				return null;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, <see langword="false" />.  In the default implementation of <see cref="T:Core.CustomLinkedList<T>" />, this property always returns <see langword="false" />.</returns>
		
		bool ICollection<T>.IsReadOnly
		{
			
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, <see langword="false" />.  In the default implementation of <see cref="T:Core.CustomLinkedList<T>" />, this property always returns <see langword="false" />.</returns>
		
		bool ICollection.IsSynchronized
		{
			
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.  In the default implementation of <see cref="T:Core.CustomLinkedList<T>" />, this property always returns the current instance.</returns>
		
		object ICollection.SyncRoot
		{
			
			get
			{
				if (_syncRoot == null)
				{
					Interlocked.CompareExchange<object>(ref _syncRoot, new object(), (object)null);
				}
				return _syncRoot;
			}
		}

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="T:Core.CustomLinkedList<T>" /> class that is empty.</summary>

        public DataLinkedList() : base()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:Core.CustomLinkedList<T>" /> class that contains elements copied from the specified <see cref="T:System.Collections.IEnumerable" /> and has sufficient capacity to accommodate the number of elements copied.</summary>
		/// <param name="collection">The <see cref="T:System.Collections.IEnumerable" /> whose elements are copied to the new <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="collection" /> is <see langword="null" />.</exception>
		
		public DataLinkedList(IEnumerable<T> collection) : base(collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			foreach (T item in collection)
			{
				AddLast(item);
				base.Add(item);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:Core.CustomLinkedList<T>" /> class that is serializable with the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" />.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object containing the information required to serialize the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> object containing the source and destination of the serialized stream associated with the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		protected DataLinkedList(SerializationInfo info, StreamingContext context) : base()
		{
			siInfo = info;
		}

        #endregion

        #region Add* Methods

        /// <summary>Adds a new node containing the specified value after the specified existing node in the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
        /// <param name="node">The <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> after which to insert a new <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> containing <paramref name="value" />.</param>
        /// <param name="value">The value to add to the <see cref="T:Core.CustomLinkedList<T>" />.</param>
        /// <returns>The new <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> containing <paramref name="value" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="node" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        ///   <paramref name="node" /> is not in the current <see cref="T:Core.CustomLinkedList<T>" />.</exception>

        private DataLinkedListNode<T> AddAfter(DataLinkedListNode<T> node, T value)
		{
			ValidateNode(node);
			DataLinkedListNode<T> DataLinkedListNode = new DataLinkedListNode<T>(node.list, value);
			InternalInsertNodeBefore(node.next, DataLinkedListNode);
			return DataLinkedListNode;
		}

		/// <summary>Adds the specified new node after the specified existing node in the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="node">The <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> after which to insert <paramref name="newNode" />.</param>
		/// <param name="newNode">The new <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> to add to the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="newNode" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> is not in the current <see cref="T:Core.CustomLinkedList<T>" />.  
		/// -or-  
		/// <paramref name="newNode" /> belongs to another <see cref="T:Core.CustomLinkedList<T>" />.</exception>

		private void AddAfter(DataLinkedListNode<T> node, DataLinkedListNode<T> newNode)
		{
			ValidateNode(node);
			ValidateNewNode(newNode);
			InternalInsertNodeBefore(node.next, newNode);
			newNode.list = this;
		}

		/// <summary>Adds a new node containing the specified value before the specified existing node in the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="node">The <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> before which to insert a new <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> containing <paramref name="value" />.</param>
		/// <param name="value">The value to add to the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <returns>The new <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> containing <paramref name="value" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> is not in the current <see cref="T:Core.CustomLinkedList<T>" />.</exception>

		private DataLinkedListNode<T> AddBefore(DataLinkedListNode<T> node, T value)
		{
			ValidateNode(node);
			DataLinkedListNode<T> DataLinkedListNode = new DataLinkedListNode<T>(node.list, value);
			InternalInsertNodeBefore(node, DataLinkedListNode);
			if (node == head)
			{
				head = DataLinkedListNode;
			}
			return DataLinkedListNode;
		}

		/// <summary>Adds the specified new node before the specified existing node in the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="node">The <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> before which to insert <paramref name="newNode" />.</param>
		/// <param name="newNode">The new <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> to add to the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="newNode" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> is not in the current <see cref="T:Core.CustomLinkedList<T>" />.  
		/// -or-  
		/// <paramref name="newNode" /> belongs to another <see cref="T:Core.CustomLinkedList<T>" />.</exception>

		private void AddBefore(DataLinkedListNode<T> node, DataLinkedListNode<T> newNode)
		{
			ValidateNode(node);
			ValidateNewNode(newNode);
			InternalInsertNodeBefore(node, newNode);
			newNode.list = this;
			if (node == head)
			{
				head = newNode;
			}
		}

		/// <summary>Adds a new node containing the specified value at the start of the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="value">The value to add at the start of the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <returns>The new <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> containing <paramref name="value" />.</returns>

		private DataLinkedListNode<T> AddFirst(T value)
		{
			DataLinkedListNode<T> DataLinkedListNode = new DataLinkedListNode<T>(this, value);
			if (head == null)
			{
				InternalInsertNodeToEmptyList(DataLinkedListNode);
			}
			else
			{
				InternalInsertNodeBefore(head, DataLinkedListNode);
				head = DataLinkedListNode;
			}
			return DataLinkedListNode;
		}

		/// <summary>Adds the specified new node at the start of the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="node">The new <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> to add at the start of the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> belongs to another <see cref="T:Core.CustomLinkedList<T>" />.</exception>

		private void AddFirst(DataLinkedListNode<T> node)
		{
			ValidateNewNode(node);
			if (head == null)
			{
				InternalInsertNodeToEmptyList(node);
			}
			else
			{
				InternalInsertNodeBefore(head, node);
				head = node;
			}
			node.list = this;
		}

		/// <summary>Adds a new node containing the specified value at the end of the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="value">The value to add at the end of the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <returns>The new <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> containing <paramref name="value" />.</returns>

		private DataLinkedListNode<T> AddLast(T value)
		{
			DataLinkedListNode<T> DataLinkedListNode = new DataLinkedListNode<T>(this, value);
			if (head == null)
			{
				InternalInsertNodeToEmptyList(DataLinkedListNode);
			}
			else
			{
				InternalInsertNodeBefore(head, DataLinkedListNode);
			}
			return DataLinkedListNode;
		}

		/// <summary>Adds the specified new node at the end of the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="node">The new <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> to add at the end of the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> belongs to another <see cref="T:Core.CustomLinkedList<T>" />.</exception>

		private void AddLast(DataLinkedListNode<T> node)
		{
			ValidateNewNode(node);
			if (head == null)
			{
				InternalInsertNodeToEmptyList(node);
			}
			else
			{
				InternalInsertNodeBefore(head, node);
			}
			node.list = this;
		}

		#endregion

		#region Remove*

		/// <summary>Removes the specified node from the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="node">The <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> to remove from the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> is not in the current <see cref="T:Core.CustomLinkedList<T>" />.</exception>

		private void Remove(DataLinkedListNode<T> node)
		{
			ValidateNode(node);
			InternalRemoveNode(node);
		}

		/// <summary>Removes the node at the start of the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:Core.CustomLinkedList<T>" /> is empty.</exception>

		private void RemoveFirst()
		{
			if (head == null)
			{
				throw new InvalidOperationException("LinkedListEmpty");
			}
			InternalRemoveNode(head);
		}

		/// <summary>Removes the node at the end of the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:Core.CustomLinkedList<T>" /> is empty.</exception>

		private void RemoveLast()
		{
			if (head == null)
			{
				throw new InvalidOperationException("LinkedListEmpty");
			}
			InternalRemoveNode(head.prev);
		}

		#endregion

		#region New Method Overrides

		/// <summary>Removes all nodes from the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		//DONE
		public new void Clear()
		{
            DataLinkedListNode<T> next = head;
			while (next != null)
			{
				DataLinkedListNode<T> DataLinkedListNode = next;
				next = next.Next;
				DataLinkedListNode.Invalidate();
			}
			head = null;
			count = 0;
			version++;
			base.Clear();
		}
		/// <summary>Removes all nodes from the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		//DONE
		public new void ClearItems()
        {
			this.Clear();
        }

		/// <summary>Determines whether a value is in the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="value">The value to locate in the <see cref="T:Core.CustomLinkedList<T>" />. The value can be <see langword="null" /> for reference types.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is found in the <see cref="T:Core.CustomLinkedList<T>" />; otherwise, <see langword="false" />.</returns>
		//DONE
		public new bool Contains(T value)
		{
			//base.Contains(value);
			//Find(value) != null;
			if ((Find(value) != null) != (base.Contains(value))) throw new DataMisalignedException();
            return base.Contains(value);
		}

		/// <summary>Copies the entire <see cref="T:Core.CustomLinkedList<T>" /> to a compatible one-dimensional <see cref="T:System.Array" />, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:Core.CustomLinkedList<T>" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:Core.CustomLinkedList<T>" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
		//DONE
		public new void CopyTo(T[] array, int index)
		{
			base.CopyTo(array, index);
            //         if (array == null)
            //{
            //	throw new ArgumentNullException("array");
            //}
            //if (index < 0 || index > array.Length)
            //{
            //	throw new ArgumentOutOfRangeException("index", index, "IndexOutOfRange");
            //}
            //if (array.Length - index < Count)
            //{
            //	throw new ArgumentException("Arg_InsufficientSpace");
            //}
            //DataLinkedListNode<T> next = head;
            //if (next != null)
            //{
            //	do
            //	{
            //		array[index++] = next.item;
            //		next = next.next;
            //	}
            //	while (next != head);
            //}
        }

		/// <summary>Adds an item at the end of the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
		/// <param name="value">The value to add at the end of the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		//DONE
		void ICollection<T>.Add(T value)
		{
			this.Add(value);
		}

		/// <summary>Adds an item at the end of the <see cref="T:System.Collections.Generic.ICollection" />.</summary>
		/// <param name="value">The value to add at the end of the <see cref="T:System.Collections.Generic.ICollection" />.</param>
		//DONE
		public new void Add(T value)
		{
			this.AddLast(value);
			base.Add(value);
		}

		/// <summary>Adds an item at the given index (Before the existing item) <see cref="T:System.Collections.Generic.ICollection" />.</summary>
		/// <param name="value">The value to add at the specified index in the <see cref="T:System.Collections.Generic.ICollection" />.</param>
		//DONE
		public new void Insert(int index, T value)
		{
			AddBefore(this.NodeAtIndex(index), value);
			base.Insert(index, value);
		}

		/// <summary>Adds an item at the given index (Before the existing item) <see cref="T:System.Collections.Generic.ICollection" />.</summary>
		/// <param name="value">The value to add at the specified index in the <see cref="T:System.Collections.Generic.ICollection" />.</param>
		//DONE
		public new void InsertItem(int index, T value)
		{
			AddBefore(this.NodeAtIndex(index), value);
			base.Insert(index, value);
		}

		/// <summary>Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.ObjectModel.Collection`1" />.</summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1" />. The value can be <see langword="null" /> for reference types.</param>
		/// <returns>The zero-based index of the first occurrence of <paramref name="item" /> within the entire <see cref="T:System.Collections.ObjectModel.Collection`1" />, if found; otherwise, -1.</returns>
		//DONE
		public new int IndexOf(T value)
		{
			return base.IndexOf(value);
		}

		/// <summary>Moves the item at the specified index to a new location in the collection.</summary>
		/// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
		/// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
		//DONE
		public new void Move(int oldIndex, int newIndex)
		{
            if (oldIndex >= Count || newIndex >= Count || oldIndex < 0 || newIndex < 0) throw new ArgumentOutOfRangeException();
            T item = base.Items[oldIndex];
            RemoveAt(oldIndex);
            Insert(newIndex, item);
			base.Move(oldIndex, newIndex);
        }

		/// <summary>Moves the item at the specified index to a new location in the collection.</summary>
		/// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
		/// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
		//DONE
		public new void MoveItem(int oldIndex, int newIndex)
		{
			this.Move(oldIndex, newIndex);
		}

		/// <summary>Replaces the element at the specified index.</summary>
		/// <param name="index">The zero-based index of the element to replace.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		//DONE
		public new void SetItem(int index, T value)
		{
			if (index >= Count || index < 0) throw new ArgumentOutOfRangeException();
            RemoveAt(index);
            Insert(index, value);
            base.SetItem(index, value);
        }

		/// <summary>Removes the first occurrence of the specified value from the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="value">The value to remove from the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <returns>
		///   <see langword="true" /> if the element containing <paramref name="value" /> is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="value" /> was not found in the original <see cref="T:Core.CustomLinkedList<T>" />.</returns>
		//DONE
		public new bool Remove(T value)
		{
			DataLinkedListNode<T> DataLinkedListNode = Find(value);
			if (DataLinkedListNode != null)
			{
				InternalRemoveNode(DataLinkedListNode);				
				return true && base.Remove(value);
			}
			return false;
		}

		/// <summary>Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1" />.</summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.  
		/// -or-  
		/// <paramref name="index" /> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.</exception>
		//DONE
		public new void RemoveAt(int index)
        {
            DataLinkedListNode<T> DataLinkedListNode = Find(base.Items[index]);
			if (DataLinkedListNode != null)
			{
				InternalRemoveNode(DataLinkedListNode);
				base.RemoveAt(index);
			}
			else throw new NullReferenceException();
		}

		/// <summary>Removes the item at the specified index of the collection.</summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		//DONE
		public new void RemoveItem(int index)
		{
			DataLinkedListNode<T> DataLinkedListNode = Find(base.Items[index]);
			if (DataLinkedListNode != null)
			{
				InternalRemoveNode(DataLinkedListNode);
				base.RemoveItem(index);
			}
			else throw new NullReferenceException();
		}

		#endregion

		#region Lookup Methods

		/// <summary>
		/// Gets the <see cref="T:Core.DataLinkedListNode<T>"/> at the given Index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>

		public DataLinkedListNode<T> NodeAtIndex(int index)
		{
			return this.Find(base.Items[index]);
		}

		/// <summary>
		/// Gets the item at the given Index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>

		public T ItemAtIndex(int index)
		{
			return this.Find(base.Items[index]).Value;
		}

		/// <summary>
		/// Gets the item Guid at the given Index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns>Returns the Guid of the item at the specified index</returns>
		
		public Guid IDAtIndex(int index)
		{
			return this.Find(base.Items[index]).Value.ID;
		}

		/// <summary>Determines whether a value is in the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <param name="value">The value to locate in the <see cref="T:Core.CustomLinkedList<T>" />. The value can be <see langword="null" /> for reference types.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is found in the <see cref="T:Core.CustomLinkedList<T>" />; otherwise, <see langword="false" />.</returns>

		public bool Contains(Guid id)
		{
			return (Find(id) != null);
		}

		/// <summary>Finds the first node that contains the specified value.</summary>
		/// <param name="value">The value to locate in the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <returns>The first <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> that contains the specified value, if found; otherwise, <see langword="null" />.</returns>

		public DataLinkedListNode<T> Find(T value)
		{
			return this.Find(value.ID);
		}
        
		/// <summary>Finds the first node that contains the specified value.</summary>
		/// <param name="value">The value to locate in the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <returns>The first <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> that contains the specified value, if found; otherwise, <see langword="null" />.</returns>
		
		public DataLinkedListNode<T> Find(Guid id)
		{
			DataLinkedListNode<T> next = head;
			if (next != null)
			{
				if (id != Guid.Empty)
				{
					do
					{
						if (next.item != null)
						{
							if (Guid.Equals(next.item.ID, id))
							{
								return next;
							}
							next = next.next;
						}
					}
					while (next != head);
				}
				else
				{
					do
					{
						if (next.item == null)
						{
							return next;
						}
						next = next.next;
					}
					while (next != head);
				}
			}
			return null;
		}

		/// <summary>Finds the last node that contains the specified value.</summary>
		/// <param name="value">The value to locate in the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <returns>The last <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> that contains the specified value, if found; otherwise, <see langword="null" />.</returns>

		public DataLinkedListNode<T> FindLast(T value)
		{
			return this.FindLast(value.ID);
		}

		/// <summary>Finds the last node that contains the specified value.</summary>
		/// <param name="value">The value to locate in the <see cref="T:Core.CustomLinkedList<T>" />.</param>
		/// <returns>The last <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> that contains the specified value, if found; otherwise, <see langword="null" />.</returns>

		public DataLinkedListNode<T> FindLast(Guid id)
		{
			if (head == null)
			{
				return null;
			}
			DataLinkedListNode<T> prev = head.prev;
			DataLinkedListNode<T> DataLinkedListNode = prev;
			if (DataLinkedListNode != null)
			{
				if (id != Guid.Empty)
				{
					do
					{
						if (Guid.Equals(DataLinkedListNode.item.ID, id))
						{
							return DataLinkedListNode;
						}
						DataLinkedListNode = DataLinkedListNode.prev;
					}
					while (DataLinkedListNode != prev);
				}
				else
				{
					do
					{
						if (DataLinkedListNode.item == null)
						{
							return DataLinkedListNode;
						}
						DataLinkedListNode = DataLinkedListNode.prev;
					}
					while (DataLinkedListNode != prev);
				}
			}
			return null;
		}

        #endregion

        #region Internal Methods

        private void InternalInsertNodeBefore(DataLinkedListNode<T> node, DataLinkedListNode<T> newNode)
		{
			newNode.next = node;
			newNode.prev = node.prev;
			node.prev.next = newNode;
			node.prev = newNode;
			version++;
			count++;
		}

		private void InternalInsertNodeToEmptyList(DataLinkedListNode<T> newNode)
		{
			newNode.next = newNode;
			newNode.prev = newNode;
			head = newNode;
			version++;
			count++;
		}

		internal void InternalRemoveNode(DataLinkedListNode<T> node)
		{
			if (node.next == node)
			{
				head = null;
			}
			else
			{
				node.next.prev = node.prev;
				node.prev.next = node.next;
				if (head == node)
				{
					head = node.next;
				}
			}
			node.Invalidate();
			count--;
			version++;
		}

		internal void ValidateNewNode(DataLinkedListNode<T> node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node.list != null)
			{
				throw new InvalidOperationException("LinkedListNodeIsAttached");
			}
		}

		internal void ValidateNode(DataLinkedListNode<T> node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node.list != this)
			{
				throw new InvalidOperationException("ExternalLinkedListNode");
			}
		}

        #endregion

        #region Inherited Implementations

        /// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="array" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index" /> is less than zero.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///   <paramref name="array" /> is multidimensional.  
        /// -or-  
        /// <paramref name="array" /> does not have zero-based indexing.  
        /// -or-  
        /// The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.  
        /// -or-  
        /// The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>

        void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Arg_MultiRank");
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException("Arg_NonZeroLowerBound");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "IndexOutOfRange");
			}
			if (array.Length - index < Count)
			{
				throw new ArgumentException("Arg_InsufficientSpace");
			}
			T[] array2 = array as T[];
			if (array2 != null)
			{
				CopyTo(array2, index);
				return;
			}
			Type elementType = array.GetType().GetElementType();
			Type typeFromHandle = typeof(T);
			if (!elementType.IsAssignableFrom(typeFromHandle) && !typeFromHandle.IsAssignableFrom(elementType))
			{
				throw new ArgumentException("Invalid_Array_Type");
			}
			object[] array3 = array as object[];
			if (array3 == null)
			{
				throw new ArgumentException("Invalid_Array_Type");
			}
			DataLinkedListNode<T> next = head;
			try
			{
				if (next != null)
				{
					do
					{
						array3[index++] = next.item;
						next = next.next;
					}
					while (next != head);
				}
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Invalid_Array_Type");
			}
		}

		/// <summary>Returns an enumerator that iterates through the linked list as a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the linked list as a collection.</returns>
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

        #endregion
    }


    /// <summary>Represents a node in a <see cref="T:Core.CustomLinkedList<T>" />. This class cannot be inherited.</summary>
    /// <typeparam name="T">Specifies the element type of the linked list.</typeparam>
    [ComVisible(false)]
	public sealed class DataLinkedListNode<T> where T : IDataGoo
	{
		internal DataLinkedList<T> list;

		internal DataLinkedListNode<T> next;

		internal DataLinkedListNode<T> prev;

		internal T item;

		/// <summary>Gets the <see cref="T:Core.CustomLinkedList<T>" /> that the <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> belongs to.</summary>
		/// <returns>A reference to the <see cref="T:Core.CustomLinkedList<T>" /> that the <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> belongs to, or <see langword="null" /> if the <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> is not linked.</returns>
		
		public DataLinkedList<T> List
		{
			
			get
			{
				return list;
			}
		}

		/// <summary>Gets the next node in the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <returns>A reference to the next node in the <see cref="T:Core.CustomLinkedList<T>" />, or <see langword="null" /> if the current node is the last element (<see cref="P:Core.CustomLinkedList<T>.Last" />) of the <see cref="T:Core.CustomLinkedList<T>" />.</returns>
		
		public DataLinkedListNode<T> Next
		{
			
			get
			{
				if (next != null && next != list.head)
				{
					return next;
				}
				return null;
			}
		}

		/// <summary>Gets the previous node in the <see cref="T:Core.CustomLinkedList<T>" />.</summary>
		/// <returns>A reference to the previous node in the <see cref="T:Core.CustomLinkedList<T>" />, or <see langword="null" /> if the current node is the first element (<see cref="P:Core.CustomLinkedList<T>.First" />) of the <see cref="T:Core.CustomLinkedList<T>" />.</returns>
		
		public DataLinkedListNode<T> Previous
		{
			
			get
			{
				if (prev != null && this != list.head)
				{
					return prev;
				}
				return null;
			}
		}

		/// <summary>Gets the value contained in the node.</summary>
		/// <returns>The value contained in the node.</returns>
		
		public T Value
		{
			
			get
			{
				return item;
			}
			
			set
			{
				item = value;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.DataLinkedListNode`1" /> class, containing the specified value.</summary>
		/// <param name="value">The value to contain in the <see cref="T:System.Collections.Generic.DataLinkedListNode`1" />.</param>
		
		public DataLinkedListNode(T value)
		{
			item = value;
		}

		internal DataLinkedListNode(DataLinkedList<T> list, T value)
		{
			this.list = list;
			item = value;
		}

		internal void Invalidate()
		{
			list = null;
			next = null;
			prev = null;
		}
	}
}
