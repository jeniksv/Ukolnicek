def prime(x):
    for i in range(2, x-1):
        if x % i == 0:
            return "NO"

    return "YES"

print( prime(int(input())) )