def prime(x):
    if x == 1:
        return "NO"

    for i in range(2, x-1):
        if x % i == 0:
            return "NO"

    return "YES"

print( prime(int(input())) )
