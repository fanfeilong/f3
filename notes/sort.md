#### 基本算法
- [排序](http://zh.wikipedia.org/wiki/%E6%8E%92%E5%BA%8F%E7%AE%97%E6%B3%95)
    - 穩定的
        - 冒泡排序(bubble sort)-O(n^2)
        - 鸡尾酒排序(cocktail sort, 雙向的冒泡排序)-O(n^2)
        - 插入排序(insertion sort)-O(n^2)
        - 桶排序(bucket sort)-O(n);需要O(k)額外空間
        - 计数排序(counting sort)-O(n+k);需要O(n+k)額外空間
        - 归并排序(merge sort)-O(n*\log_{n});需要O(n)額外空間
        - 原地归并排序- O(n^2)
        - 二叉排序树排序(binary tree sort)- O(n*\log_{n})期望时间; O(n^2)最坏时间;需要O(n)額外空間
        - 鸽巢排序(pigeonhole sort)-O(n+k);需要O(k)額外空間
        - 基數排序(radix sort)-O(n+k);需要O(n)額外空間
        - 侏儒排序(gnome sort)- O(n^2)
        - 图书馆排序(library sort)- O(n*\log_{n}) with high probability, 需要(1+ε)n額外空間

    - 不穩定
        - 選擇排序(selection sort)-O(n^2)
        - 希爾排序(shell sort)-O(n*\log_{n})如果使用最佳的現在版本
        - 组合排序- O(n*\log_{n})
        - 堆排序(heap sort)-O(n*\log_{n})
        - 平滑排序(smooth sort)- O(n*\log_{n})
        - 快速排序(quick sort)-O(n*\log_{n})期望時間,  O(n^2)最壞情況;對於大的、亂數串列一般相信是最快的已知排序
        - 內省排序(introsort)-O(n*\log_{n})
        - 耐心排序(patience sort)-O(n*\log_{n}+k)最坏情況時間，需要額外的O(n+k)空間，也需要找到最長的遞增子序列(longest increasing subsequence)
    
    - 不實用的排序算法
        - Bogo排序- O(n × n!)，最壞的情況下期望時間為無窮。
        - Stupid排序-O(n3);遞迴版本需要O(n^2)額外記憶體
        - 珠排序(bead sort)- O(n) or O(\sqrt_{n}), 但需要特別的硬體
        - Pancake sorting-O(n), 但需要特別的硬體
        - 臭皮匠排序(stooge sort)算法简单，但需要约n^2.7的时间


- 查找
    - 线性查找
    - 二分查找
    
#### 基本数据结构
- 数组
- 列表
- 栈
- 队列
- 树
    - 二叉树
        - AVLTree
        - RBTree
        - B+Tree
          - [A Lock-Free B+tree](http://www.cs.technion.ac.il/~anastas/lfbtree-spaa.pdf) 
        - B-Tree
        - B*Tree
        - SegementTree
        - TireTree
    - 多叉树
- 字符串
    - [字符串相关算法](http://en.wikipedia.org/wiki/String_searching_algorithm) 
        - Na?ve string search algorithm
        - Rabin–Karp string search algorithm
        - Finite-state automaton based search
        - Knuth–Morris–Pratt algorithm
        - Boyer–Moore string search algorithm
        - Bitap algorithm
        - Aho–Corasick string matching algorithm
        - Commentz-Walter algorithm
        - Rabin–Karp string search algorithm
        - regular expression
        - [EXACT STRING MATCHING ALGORITHMS](http://www-igm.univ-mlv.fr/~lecroq/string/)
- 图
