
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Tatacoa 
{
	public static partial class Fn {

		//Functor
		//FMap :: (a -> b) -> IEnumerable a -> IEnumerable b
		public static IEnumerable<B> FMap<A,B> (Func<A,B> f, IEnumerable<A> F) {
			return F.Select (f);
		}

		//FMap :: (a -> b) -> (IEnumerable a -> IEnumerable b)
		public static Func<IEnumerable<A>,IEnumerable<B>> FMap<TList,A,B> (Func<A,B> f) {
			return F => F.Select (f);
		}

		//FMap :: (a -> void) -> IEnumerable a -> IEnumerable b
		public static IEnumerable<A> FMap<A> (Action<A> f, IEnumerable<A> F) {
			return F.Select (f.ToFunc());
		}

		//FMap :: (a -> b) -> (IEnumerable a -> IEnumerable b)
		public static Func<IEnumerable<A>,IEnumerable<A>> FMap<TList,A> (Action<A> f) {
			return F => FMap (f, F);
		}

		public static IEnumerable<B> FMap<A,B> (this IEnumerable<A> e, Func<A,B> f) {
			return FMap (f, e);
		}

		public static IEnumerable<A> FMap<A> (this IEnumerable<A> e, Action<A> f) {
			return FMap (f.ToFunc(), e);
		}

		public static IEnumerable<A> FMap<A> (this IEnumerable<A> e, Action f) {
			return FMap (f.ToFunc<A>(), e);
		}

		//Applicative
		//Pure :: a -> [a]
		public static IEnumerable<A> Pure<A> (TList _, A a) {
			return Fn.Repeat (a);
		}

		//Pure :: a -> [a]
		public static IEnumerable<A> Pure<A> (this IEnumerable<A> l, A a) {
			return Fn.Repeat (a);
		}

		// Apply [a -> b] -> [a] -> [b]
		public static IEnumerable<B> Apply<A,B> (this IEnumerable<Func<A,B>> ef, IEnumerable<A> e) {
			foreach (var f in ef) {
				foreach (var a in e) {
					yield return f (a);
				}	
			}
		}

		// Apply [a -> b] -> [a] -> [b]
		public static Func <IEnumerable<A>, IEnumerable<B>> Apply<A,B> (this IEnumerable<Func<A,B>> ef) {
			return e => ef.Apply (e);
		}
		
		//Monda

		//Bind :: (a -> [b]) -> [a] -> [b]
		public static IEnumerable<B> Bind<A,B> (Func <A, IEnumerable<B>> f, IEnumerable<A> e) {
			return Concat (FMap (f, e));
		}

		//Bind :: (a -> [b]) -> [a] -> [b]
		public static Func <IEnumerable<A>, IEnumerable<B>> Bind<A,B> (Func <A, IEnumerable<B>> f) {
			return e => Bind (f, e);
		}

		//Bind :: (a -> [b]) -> [a] -> [b]
		public static IEnumerable<B> Bind<A,B> (this IEnumerable<A> e, Func <A, IEnumerable<B>> f) {
			return Bind (f, e);
		}

		public static IEnumerable<A> IfFailed<A> (this IEnumerable<A> e, Action f) {
			var enu = e.GetEnumerator ();

			if (! enu.MoveNext ()) {
				f ();	
			}
			else {
				yield return enu.Current;

				while (enu.MoveNext()) {
					yield return enu.Current;	
				}
			}

		}
	}

	public static partial class Fn {

		// Map :: (a -> b) -> [a] -> [b]
		public static List<B> Map<A,B> (Func<A,B> f, List<A> e) {
			return FMap (f, e).ToList ();
		}
		// Map :: (a -> b) -> ([a] -> [b])
		public static Func<List<A>,List<B>> Map<A,B> (Func<A,B> f) {
			return e => FMap (f, e).ToList ();
		}
		// Map :: ((a -> b) -> ([a] -> [b]))
		public static Func<Func<A,B>,Func<List<A>,List<B>>> Map<A,B> () {
			return f => e => FMap (f, e).ToList ();
		}

		// Map :: (a -> b) -> [a] -> [b]
		public static List<B> Map<A,B> (this List<A> e, Func<A,B> f) {
			return Map (f, e);
		}

		// Map :: (a -> void) -> [a] -> [a]
		public static List<A> Map<A> (Action<A> f, List<A> e) {
			return Map (f.ToFunc(), e);
		}
		// Map :: (a -> void) -> ([a] -> [a])
		public static Func<List<A>,List<A>> Map<A> (Action<A> f) {
			return e => Map (f, e);
		}
		// Map :: ((a -> void) -> ([a] -> [a]))
		public static Func<Action<A>,Func<List<A>,List<A>>> Map<A> () {
			return f => e => Map (f, e);
		}
		// Map :: (a -> void) -> [a] -> [a]
		public static List<A> Map<A> (this List<A> e, Action<A> f) {
			return Map (f, e);
		}

		public static List<B> Map<A,B> (this IEnumerable<A> e, Func<A,B> f) {
			return e.ToList ().Map (f);
		}

		public static List<A> Map<A> (this IEnumerable<A> e, Action<A> f) {
			return e.ToList ().Map (f.ToFunc());
		}

		public static void ForEach<A> (this IEnumerable<A> e, Action<A> f)
		{
			foreach (var a in e) 
				f(a);
		}

		//Filter :: (a -> bool) -> [a] -> [a]
		public static IEnumerable<A> Filter<A> (Func<A,bool> cond, IEnumerable<A> e) {
			return e.Where (cond);
		}

		//Filter :: (a -> bool) -> [a] -> [a]
		public static Func<IEnumerable<A>,IEnumerable<A>> Filter<A> (Func<A,bool> cond) {
			return e => Filter (cond, e);
		}

		//Filter :: (a -> bool) -> [a] -> [a]
		public static Func <Func<A,bool>, IEnumerable<A>, IEnumerable<A>> Filter<A> () {
			return (cond, e) => Filter (cond, e);
		}

		//Filter :: (a -> bool) -> [a] -> [a]
		public static IEnumerable<A> Filter<A> (this IEnumerable<A> e, Func<A,bool> cond) {
			return e.Where (cond);
		}

		// Reverse :: [a] -> [a]
		public static IEnumerable<A> Reverse<A> (IEnumerable<A> e) {
			return e.Reverse ();
		}

		// Reverse :: ([a] -> [a])
		public static Func<IEnumerable<A>,IEnumerable<A>> Reverse<A> () {
			return e => e.Reverse ();
		}

		// FoldL :: (a -> b -> a) -> a -> [b] -> a
		public static A FoldL<A,B> (Func<A,B,A> f, A a, IEnumerable<B> e) {
			return e.Aggregate (a, f);
		}

		// FoldL :: (a -> b -> a) -> a -> ([b] -> a)
		public static Func<IEnumerable<B>,A> FoldL<A,B>  (Func<A,B,A> f, A a) {
			return e => FoldL (f, a, e);
		}

		// FoldL :: (a -> b -> a) -> (a -> ([b] -> a))
		public static Func<A,Func<IEnumerable<B>,A>> FoldL<A,B>  (Func<A,B,A> f) {
			return a => e => FoldL (f, a, e);
		}

		// FoldL :: ((a -> b -> a) -> (a -> ([b] -> a)))
		public static Func<Func<A,B,A>,Func<A,Func<IEnumerable<B>,A>>> FoldL<A,B>  () {
			return f => a => e => FoldL (f, a, e);
		}

		// FoldL :: (a -> b -> a) -> a -> [b] -> a
		public static A FoldL<A,B> (this IEnumerable<B> e, A a, Func<A,B,A> f) {
			return FoldL (f, a, e);
		}

		// FoldL1 :: (a -> a -> a) -> [a] -> a
		public static A FoldL1<A> (Func<A,A,A> f, IEnumerable<A> e) {
			var enu = e.GetEnumerator ();
			enu.MoveNext ();
			var acc = enu.Current;

			while (enu.MoveNext()) {
				acc = f (acc, enu.Current);	
			}

			return acc;
		}

		// FoldL1 :: (a -> a -> a) -> [a] -> a
		public static Func<IEnumerable<A>,A> FoldL1<A> (Func<A,A,A> f) {
			return e => FoldL1 (f, e);
		}

		// FoldL1 :: (a -> a -> a) -> [a] -> a
		public static Func<Func<A,A,A>,Func<IEnumerable<A>,A>> FoldL1<A> () {
			return f => e => FoldL1 (f, e);
		}

		// FoldL1 :: (a -> a -> a) -> [a] -> a
		public static A FoldL1<A> (this IEnumerable<A> e, Func<A,A,A> f) {
			return FoldL1 (f, e);
		}

		// ZipWith :: (a -> b -> c) -> [a] -> [b] -> [c]
		public static IEnumerable<C> ZipWith<A,B,C> (Func<A,B,C> f, IEnumerable<A> ea, IEnumerable<B> eb) {
			var a = ea.GetEnumerator ();
			var b = eb.GetEnumerator ();

			while (b.MoveNext() && a.MoveNext()) {
				yield return f (a.Current, b.Current);
			}
		}

		// ZipWith :: (a -> b -> c) -> ([a] -> ([b] -> [c]))
		public static Func<IEnumerable<A>,Func<IEnumerable<B>,IEnumerable<C>>> ZipWith<A,B,C> (Func<A,B,C> f) {
			return ea => eb => ZipWith (f, ea, eb);
		}

		// ZipWith :: ((a -> b -> c) -> ([a] -> ([b] -> [c])))
		public static Func<Func<A,B,C>,Func<IEnumerable<A>,Func<IEnumerable<B>,IEnumerable<C>>>> ZipWith<A,B,C> () {
			return f => ea => eb => ZipWith (f, ea, eb);
		}

		// ZipWith :: (a -> b -> c) -> [a] -> [b] -> [c]
		public static IEnumerable<B> ZipWith<A,B> (Action<A,B> f, IEnumerable<A> ea, IEnumerable<B> eb) {
			return ZipWith (f.ToFunc (), ea, eb);  
		}

		// ZipWith :: [a] -> [b] -> (a -> b -> c) -> [c]
		public static IEnumerable<C> ZipWith<A,B,C> (this IEnumerable<A> ea, IEnumerable<B> eb, Func<A,B,C> f) {
			return ZipWith (f, ea, eb);
		}

		// ZipWith :: [a] -> [b] -> (a -> b -> c) -> [c]
		public static IEnumerable<B> ZipWith<A,B> (this IEnumerable<A> ea, IEnumerable<B> eb, Action<A,B> f) {
			return ZipWith (f.ToFunc(), ea, eb);
		}

		// ZipProd :: (a -> b -> c) -> [a] -> [b] -> [c]
		public static IEnumerable<C> ZipProd<A,B,C> (Func<A,B,C> f, IEnumerable<A> ea, IEnumerable<B> eb) {
			foreach (var a in ea) {
				foreach (var b in eb) {
					yield return f (a, b);
				}	
			}
		}

		// ZipProd :: (a -> b -> c) -> ([a] -> ([b] -> [c]))
		public static Func<IEnumerable<A>,Func<IEnumerable<B>,IEnumerable<C>>> ZipProd<A,B,C> (Func<A,B,C> f) {
			return ea => eb => ZipProd (f, ea, eb);
		}
		
		// ZipProd :: ((a -> b -> c) -> ([a] -> ([b] -> [c])))
		public static Func<Func<A,B,C>,Func<IEnumerable<A>,Func<IEnumerable<B>,IEnumerable<C>>>> ZipProd<A,B,C> () {
			return f => ea => eb => ZipProd (f, ea, eb);
		}
		
		// ZipProd :: (a -> b -> void) -> [a] -> [b] -> [b]
		public static IEnumerable<B> ZipProd<A,B> (Action<A,B> f, IEnumerable<A> ea, IEnumerable<B> eb) {
			return ZipProd (f.ToFunc (), ea, eb);  
		}

		// ScanL :: :: (a -> a -> a) -> [a] -> [a]
		public static IEnumerable<A> ScanL1<A> (Func<A,A,A> f, IEnumerable<A> e) {
			A a;

			var num = e.GetEnumerator ();
			num.MoveNext ();
			yield return (a = num.Current);

			while (num.MoveNext()) {
				yield return (a = f (a, num.Current));
			}
		}

		// ScanL :: :: (a -> b -> a) -> a -> [b] -> a
		public static IEnumerable<A> ScanL<A,B> (Func<A,B,A> f, A a, IEnumerable<B> e) {
			yield return a;
			foreach (var b in e) {
				yield return (a = f (a, b));
			}
		}

		// Repeat :: a -> [a]
		// a.Repeat :: [a]
		public static IEnumerable<A> Repeat<A> (this A a) {
			while (true)
				yield return a;
		}

		public static IEnumerable<A> Repeat<A> (this Func<A> f) {
			while (true)
				yield return f();
		}

		public static IEnumerable Repeat (this Action f) {
			while (true) {
				f();
				yield return null;
			}
				
		}



		// Take :: int -> [a] -> [a]
		public static IEnumerable<A> Take<A> (int n, IEnumerable<A> e) {
			return e.Take (n);
		}

		// Take :: int -> ([a] -> [a])
		public static Func<IEnumerable<A>,IEnumerable<A>> Take<A> (int n) {
			return e => Take (n, e);
		}

		// Take :: (int -> ([a] -> [a]))
		public static Func<int,IEnumerable<A>,IEnumerable<A>> Take<A> () {
			return (n, e) => Take (n, e);
		}

		// TakeWhile :: (a -> bool) -> [a] -> [a]
		public static IEnumerable<A> TakeWhile<A> (Func<A,bool> f, IEnumerable<A> e) {
			var enu = e.GetEnumerator ();

			while (enu.MoveNext() && f (enu.Current)) {
				yield return enu.Current;
			}
		}

		// TakeWhile :: (a -> bool) -> [a] -> [a]
		public static Func<IEnumerable<A>,IEnumerable<A>> TakeWhile<A> (Func<A,bool> f) {
			return e => TakeWhile (f, e);
		}

		// TakeWhile :: (a -> bool) -> [a] -> [a]
		public static Func<Func<A,bool>,IEnumerable<A>,IEnumerable<A>> TakeWhile<A> () {
			return (f, e) => TakeWhile (f, e);
		}

		// TakeWhile :: (a -> bool) -> [a] -> [a]
		public static IEnumerable<A> TakeWhile<A> (Func<A,bool> f, IEnumerator<A> enu) {
			
			while (enu.MoveNext() && f (enu.Current)) {
				yield return enu.Current;
			}
		}

		public static IEnumerable<IEnumerable<A>> SplitWhen<A> (Func<A,bool> f, IEnumerable<A> e) {
			var enu = e.GetEnumerator ();
			var l = new LinkedList<A> ();

			while (enu.MoveNext()) {
				if (f (enu.Current)) {
					yield return l;
					l.Clear();
				}
				else {
					l.AddLast (enu.Current);
				}
			}
		}

		public static IEnumerable<IEnumerable<A>> SplitEvery<A> (int n, IEnumerable<A> e) {
			var enu = e.GetEnumerator ();
			var l = new LinkedList<A> ();

			var i = 0;
			while (enu.MoveNext()) {

				l.AddLast (enu.Current);

				if (++i == n) {
					yield return l;
					l = new LinkedList<A> ();
					i = 0;
				}
			}

			if (i != 0)
				yield return l;
		}

		public static IEnumerable<IEnumerable<A>> SplitEvery<A> (this IEnumerable<A> e, int n) {
			return SplitEvery (n, e);
		}

		// Replicate :: int -> a -> [a]
		public static IEnumerable<A> Replicate<A> (int n, A a) {
			return a.Repeat ().Take (n);
		}

		// Replicate :: int -> (a -> [a])
		public static Func<A,IEnumerable<A>> Replicate<A> (int n) {
			return a => Replicate (n, a);
		}

		// Replicate :: (int -> (a -> [a]))
		public static Func<int,Func<A,IEnumerable<A>>> Replicate<A> () {
			return n => a => Replicate (n, a);
		}

		// Replicate :: int -> a -> [a]
		public static IEnumerable<A> Replicate<A> (this A a, int n) {
			return Replicate (n, a);
		}

		// Drop :: int -> [a] -> [a]
		public static IEnumerable<A> Drop<A> (int n, IEnumerable<A> e) {
			return e.Skip (n);
		}
		
		// Take :: int -> ([a] -> [a])
		public static Func<IEnumerable<A>,IEnumerable<A>> Drop<A> (int n) {
			return e => Drop (n, e);
		}
		
		// Take :: (int -> ([a] -> [a]))
		public static Func<int,Func<IEnumerable<A>,IEnumerable<A>>> Drop<A> () {
			return n => e => Drop (n, e);
		}

		// Drop :: [a] -> int -> [a]
		public static IEnumerable<A> Drop<A> (this IEnumerable<A> e, int n) {
			return e.Skip (n);
		}

		// Slice :: int -> int -> [a] -> [a]
		public static IEnumerable<A> Slice<A> (int from, int to, IEnumerable<A> e) {
			return e.Take (to).Drop (from);
		}

		// Slice :: int -> (int -> ([a] -> [a]))
		public static Func<int,Func<IEnumerable<A>,IEnumerable<A>>> Slice<A> (int from) {
			return to => e => Slice(from, to, e);
		}

		// Slice :: (int -> (int -> ([a] -> [a])))
		public static Func<int,Func<int,Func<IEnumerable<A>,IEnumerable<A>>>> Slice<A> () {
			return from => to => e => Slice(from, to, e);
		}

		// Slice :: int -> int -> [a] -> [a]
		public static IEnumerable<A> Slice<A> (this IEnumerable<A> e, int from, int to) {
			return Slice (from, to, e);
		}

		// Iterate :: (a -> a) -> a -> [a]
		public static IEnumerable<A> Iterate<A> (Func<A,A> f, A a) {
			while (true) {
				yield return a;
				a = f(a);
			}
		}

		// Iterate :: (a -> a) -> (a -> [a])
		public static Func<A,IEnumerable<A>> Iterate<A> (Func<A,A> f) {
			return a => Iterate (f, a);
		}

		// a.Iterate :: (a -> a) -> [a]
		public static IEnumerable<A> Iterate<A> (this A a, Func<A,A> f) {
			return Iterate (f, a);
		}

		// IterateWhile :: (a -> bool) -> (a -> a) -> a -> [a]
		public static IEnumerable<A> IterateWhile<A> (Func<A,bool> cond, Func<A,A> f, A a) {
			return TakeWhile (cond, Iterate (f, a));
		}

		// IterateWhile :: (a -> bool) -> (a -> a) -> a -> [a]
		public static Func<Func<A,A>, A, IEnumerable<A>> IterateWhile<A> (Func<A,bool> cond) {
			return (f, a) => TakeWhile (cond, Iterate (f, a));
		}

		// IterateWhile :: (a -> bool) -> (a -> a) -> a -> [a]
		public static Func<Func<A,bool> ,Func<A,A>, A, IEnumerable<A>> IterateWhile<A> () {
			return (cond, f, a) => TakeWhile (cond, Iterate (f, a));
		}

		// IterateWhile :: (a -> bool) -> (a -> a) -> a -> [a]
		public static IEnumerable<A> IterateWhile<A> (this A a, Func<A,bool> cond, Func<A,A> f) {
			return TakeWhile (cond, Iterate (f, a));
		}

		public static IEnumerable<A> Cycle<A> (this IEnumerable<A> e){
			while (true)
				foreach(var a in e) 
					yield return a;
		}

		public static Func<IEnumerable<A>,IEnumerable<A>> Cycle<A> () {
			return e => Cycle (e);
		}

		public static IEnumerable Cycle (this IEnumerable e){
			while (true)
				foreach(var _ in e) 
					yield return _;
		}

		//Join :: [a] -> [a] -> [a]
		public static IEnumerable<A> Join<A> (this IEnumerable<A> a, IEnumerable<A> b) {
			var enuA = a.GetEnumerator ();
			var enuB = b.GetEnumerator ();

			while (enuA.MoveNext())
				yield return enuA.Current;

			while (enuB.MoveNext())
				yield return enuB.Current;
		}

		//Join :: [a] -> [a] -> [a]
		public static Func<IEnumerable<A>,IEnumerable<A>> Join<A> (this IEnumerable<A> a) {
			return b => Join (a, b);
		}

		//Join :: [a] -> [a] -> [a]
		public static Func<IEnumerable<A>,Func<IEnumerable<A>,IEnumerable<A>>> Join<A> () {
			return a => b => Join (a, b);
		}

		//Join :: [a] -> [a] -> [a]
		public static IEnumerable Join (this IEnumerable a, IEnumerable b)
		{
			foreach (var _ in a) yield return _;

			foreach (var _ in b) yield return _;
		}

		//Join :: [a] -> [a] -> [a]
		public static Func<IEnumerable,IEnumerable> Join (IEnumerable a) {
			return b => Join (a, b);
		}

		//Join :: [a] -> [a] -> [a]
		public static Func<IEnumerable,Func<IEnumerable,IEnumerable>> Join () {
			return a => b => Join (a, b);
		}

		//Join :: [a] -> [a] -> [a]
		public static IEnumerable<A> Join<A> (this IEnumerable<A> a, Func<IEnumerable<A>> b) {

			var enuA = a.GetEnumerator ();
			while (enuA.MoveNext())
				yield return enuA.Current;

			var enuB = b().GetEnumerator ();
			while (enuB.MoveNext())
				yield return enuB.Current;
		}

		//Join :: [a] -> [a] -> [a]
		public static IEnumerable Join<A> (this IEnumerable a, Func<IEnumerable<A>> b) {
			
			var enuA = a.GetEnumerator ();
			while (enuA.MoveNext())
				yield return enuA.Current;
			
			var enuB = b().GetEnumerator ();
			while (enuB.MoveNext())
				yield return enuB.Current;
		}

		//Join :: [a] -> [a] -> [a]
		public static IEnumerable Join (this IEnumerable a, Func<IEnumerable> b) {
			
			var enuA = a.GetEnumerator ();
			while (enuA.MoveNext())
				yield return enuA.Current;
			
			var enuB = b().GetEnumerator ();
			while (enuB.MoveNext())
				yield return enuB.Current;
		}

		//Join :: [a] -> [a] -> [a]
		public static IEnumerable Join<A> (this IEnumerable a, Func<Seq<A>> b) {
			
			var enuA = a.GetEnumerator ();
			while (enuA.MoveNext())
				yield return enuA.Current;
			
			var enuB = b().GetEnumerator ();
			while (enuB.MoveNext())
				yield return enuB.Current;
		}

		//Join :: [a] -> [a] -> [a]
		public static IEnumerable Join<A> (this IEnumerable a, Func<A> b) {
			
			var enuA = a.GetEnumerator ();
			while (enuA.MoveNext())
				yield return enuA.Current;
			
			yield return b ();
		}

		//Concat :: [[a]] -> [a]
		public static IEnumerable<A> Concat<A> (this IEnumerable<IEnumerable<A>> listOfLists) {
			return FoldL1 (Join<A> ().Uncurry (), listOfLists);
		}

		//Concat :: [[a]] -> [a]
		public static Func<IEnumerable<IEnumerable<A>>,IEnumerable<A>> Concat<A> () {
			return listOfLists => FoldL1 (Join<A> ().Uncurry (), listOfLists);
		}

		//Concat :: [[object]] -> [object]
		public static IEnumerable Concat (this IEnumerable<IEnumerable> listOfLists) {
			return FoldL1 (Join ().Uncurry (), listOfLists);
		}
		
		//Concat :: [[object]] -> [object]
		public static Func<IEnumerable<IEnumerable>,IEnumerable> Concat () {
			return listOfLists => FoldL1 (Join ().Uncurry (), listOfLists);
		}

		//AppendL :: a -> [a] -> [a]
		public static IEnumerable<A> AppendL<A> (this A a, IEnumerable<A> e) {
			yield return a;

			foreach (var x in e) {
				yield return x;	
			}
		}

		//AppendL :: a -> ([a] -> [a])
		public static Func<IEnumerable<A>,IEnumerable<A>> AppendL<A> (this A a) {
			return e => AppendL (a, e);
		}

		//AppendL :: (a -> ([a] -> [a]))
		public static Func<A,Func<IEnumerable<A>,IEnumerable<A>>> AppendL<A> () {
			return a => e => AppendL (a, e);
		}

		//AppendR :: a -> [a] -> [a]
		public static IEnumerable<A> AppendR<A> (this A a, IEnumerable<A> e) {
			
			foreach (var x in e) {
				yield return x;	
			}

			yield return a;
		}

		//AppendR :: a -> ([a] -> [a])F
		public static Func<IEnumerable<A>,IEnumerable<A>> AppendR<A> (this A a) {
			return e => AppendR (a, e);
		}
		
		//AppendR :: (a -> ([a] -> [a]))
		public static Func<A,Func<IEnumerable<A>,IEnumerable<A>>> AppendR<A> () {
			return a => e => AppendR (a, e);
		}

		//AppendR :: a -> [obj] -> [obj]
		public static IEnumerable AppendR<A> (this A value, IEnumerable e) {
			foreach (var x in e) {
				yield return x;	
			}
			
			yield return value;
		}
		
		//AppendR :: obj -> [obj] -> [obj]
		public static IEnumerable AppendR<A> (this Func<A> f, IEnumerable e) {
			foreach (var x in e) {
				yield return x;	
			}
			
			yield return f();
		}

		public static IEnumerable<A> Append<A> (this IEnumerable<A> e, A a) {
			return AppendR (a, e);
		}

		public static Func <A, IEnumerable<A>> Append<A> (this IEnumerable<A> e) {
			return a => AppendR (a, e);
		}

		//Head :: [a] -> a
		public static A Head<A> (this IEnumerable<A> e) {
			return e.First ();
		}

		//Head :: [a] -> a
		public static Func <IEnumerable<A>, A> Head<A> () {
			return e => e.Head ();
		}

		//MaybeHead :: [a] -> Maybe a
		public static Maybe<A> MaybeHead<A> (this IEnumerable<A> e) {
			var enu = e.GetEnumerator ();

			if (enu.MoveNext ())
				return Fn.Maybe (enu.Current);
			else
				return Fn.Nothing<A> ();
		}

		//MaybeHead :: [a] -> Maybe a
		public static Func <IEnumerable<A>, Maybe<A>> MaybeHead<A> () {
			return e => MaybeHead (e);
		}
		
		//Last :: [a] -> a
		public static Func <IEnumerable<A>, A> Last<A> () {
			return e => e.Last ();
		}
		
		//MaybeLast :: [a] -> Maybe a
		public static Maybe<A> MaybeLast<A> (this IEnumerable<A> e) {
			var enu = e.GetEnumerator ();
			
			if (enu.MoveNext ()) 
			{
				while (enu.MoveNext ()) {}

				return Fn.Maybe (enu.Current);
			}
			else
				return Fn.Nothing<A> ();
		}
		
		//MaybeLast :: [a] -> Maybe a
		public static Func <IEnumerable<A>, Maybe<A>> MaybeLast<A> () {
			return e => MaybeLast (e);
		}

		public static IEnumerable<A> Enumerate<A> (Func<A> f) {
			yield return f();
		}

		public static IEnumerable<A> Enumerate<A> (Func<IEnumerable<A>> f) {
			var enu = f().GetEnumerator();
			
			while (enu.MoveNext()) {
				yield return enu.Current;
			}
		}

		public static IEnumerable Enumerate<A> (Func<Seq<A>> f) {
			var enu = f().GetEnumerator();
			
			while (enu.MoveNext()) {
				yield return enu.Current;	
			}
		}

		public static IEnumerable Enumerate (Func<IEnumerable> f) {
			IEnumerable e = f ();
			var enu = e.GetEnumerator();
			
			while (enu.MoveNext()) {
				yield return enu.Current;	
			}
		}

		public static IEnumerable Enumerate (Action f) {
			f ();
			yield return null;
		}

		public static IEnumerable<A> Enumerate<A> (A a) {
			yield return a;
		}


		public static Func<Action,IEnumerable> Enumerate () {
			return f => Enumerate (f);
		}

		public static void Run (this IEnumerable e) {
			foreach (var _ in e) {}
		}

		public static bool Any<A> (Func<A,bool> f, IEnumerable<A> e) {
			return ! (e.Filter (f).MaybeHead ().IsNothing);
		}

		public static Func <IEnumerable<A>, bool> Any<A> (Func<A,bool> f) {
			return e => Any (f, e);
		}

		public static Func <Func<A,bool>, Func <IEnumerable<A>, bool>> Any<A> () {
			return f => e => Any (f, e);
		}

		public static bool Any<A> (this IEnumerable<A> e, Func<A,bool> f) {
			return Any (f, e);
		}

		public static bool All<A> (Func<A,bool> f, IEnumerable<A> e) {
			return ! Any (Not (f), e);
		}

		public static Func <IEnumerable<A>, bool> All<A> (Func<A,bool> f) {
			return e => All (f, e);
		}

		public static Func<Func<A,bool>,Func <IEnumerable<A>, bool>> All<A> () {
			return f => e => All (f, e);
		}

		public static bool All<A> (this IEnumerable<A> e, Func<A,bool> f) {
			return All (f, e);
		}

		public static bool And (this IEnumerable<bool> e) {
			return All (Fn.Id<bool> (), e);
		}

		public static bool Or (this IEnumerable<bool> e) {
			return Any (Fn.Id<bool> (), e);
		}

		//Shortcuts
		public static float Sum (float a, float b)
		{
			return a + b;
		}

	}

	public class TList {
		private static TList _i;
		public static TList i {
			get {
				if (_i != null) _i = new TList();
				return _i;
			}
		}
		private TList (){}
	}
}