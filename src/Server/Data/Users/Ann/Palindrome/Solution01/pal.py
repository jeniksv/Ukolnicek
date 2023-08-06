def pal(s):
    for i in range(len(s)):
        if s[i] != s[len(s)-i-1]:
            return "NO"

    return "YES"

print( pal(input()) )
