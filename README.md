CryptoTest
==========

Testes com criptografia de par de chaves assimétricas pública e privada em .NET/C#.

## EXIBIR AJUDA
```console
$ CryptoTests.exe --help
CryptoTests - Perform Key Pair Encrypt Tests

usage: CryptoTests [OPTIONS]

Options:
  --help, -h            Show this help message
  --base <BASE_PATH>    Set a base directory path.
                        > Default is "$HOME\CryptoTests"
  --message <MESSAGE>   Set a message content to encrypt/decrypt tests
                        > Default is "Test Message!"
  --pubkey <FILE_PATH>  Set a public key file path
  --privkey <FILE_PATH> Set a private key file path
  --gen, -g             Generate a encrypt key pair files on "--base" directory:
                        > key-private.pem  Private key in PEM format
                        > key-public.pem   Public key in PEM format
                        > key-private.xml  Private key in XML format
                        > key-public.xml   Public key in XML format
  --test-xml            Perform encrypt/decrypt tests with generated XML files
  --test-pem            Perform encrypt/decrypt tests with generated PEM files
  --encrypt, -e         Encrypt message
  --decrypt, -d         Decrypt message
```

## GERAR CHAVES
```console
$ CryptoTests.exe --gen
$ ls $HOME/CryptoTests
- key-private.pem
- key-private.xml
- key-public.pem
- key-public.xml
```

## GERAR CHAVES EM UMA PASTA ESPECÍFICA
```console
$ CryptoTests.exe --gen --base $HOME/MinhasChaves
$ ls $HOME/MinhasChaves
- key-private.pem
- key-private.xml
- key-public.pem
- key-public.xml
```

## EXECUTAR TESTES NOS ARQUIVOS XML/PEM GERADOS
```console
$ CryptoTests.exe --test-xml
---------------------- PERFORM XML TEST (BEGIN) ----------------------
Original message: Test Message!
Encrypted message: PaK9j85yJXhJBdzr1mcjH/fH1BmAUXoi6AEdZhQ53vVqrNwc/S6
Io6SPyQGdGV66i3Uq9Ltmp/IxGUjI7CD5BDTGGPxmNR27SJm4O5eBCGv56yVO+1MF8d8oE
09Uc8BdSk9ss1tHodSAcqXVq0MPgb4Lvb/dhGepIZzWPPCGa04MVf5lgediCcX+0V3PZSK
d5UfkRe4x6qYeSzWpIsXNAX/k0W8DiBLGQG2wy5PwVNzD1TZassJFqcRF4pNd54QJ4Pzg2
kybFWaFNy7ONDiooJ+8O/g/h7nNUK9xjjBMEdBsyodSyIiWsuBaxTtzYalbpZbiBSYiFga
wXtdxXzlcMw==
Decrypted message: Test Message!
---------------------- PERFORM XML TEST (END) ------------------------

$ CryptoTests.exe --test-pem
---------------------- PERFORM PEM TEST (BEGIN) ----------------------
Original message: Test Message!
Encrypted message: BSmrw1+UFNJkcgkJeMtPJ1JLra79fMDwxlINzeTSjQ2HEEoDAk0
GdGGQPpXGAhxL99VVDqxdXwDw/qn941GJ1HxOqo8acmRj2uqP6r//vqYLZa9mTF5n2DZfN
rpq2E7eCEKXazZkwRd5xaScNP3JnrilwJ5ZLqcHuAFfFwXgA3hlnNLSkuxtwXg1XceFsfm
WtcRHRBWh4QuA0El8VsQz+Kca9/fOdScw8dZcD/+0adZi3iuXL28jZ/BYrseonEYxh5KQr
P7CR5UBorNTi1eyzi78cbZg3/1x5bRQ6pyjjBTFMS5xM7Y4chUL7a4ShGi/KGJWvj7a2NQ
p40/DJMiBzw==
Decrypted message: Test Message!
---------------------- PERFORM PEM TEST (END) ------------------------
```

## EXECUTAR TESTES NOS ARQUIVOS XML/PEM GERADOS EM UM DIRETÓRIO ESPECÍFICO
```console
$ CryptoTests.exe --test-xml --base $HOME/MinhasChaves
---------------------- PERFORM XML TEST (BEGIN) ----------------------
Original message: Test Message!
Encrypted message: PaK9j85yJXhJBdzr1mcjH/fH1BmAUXoi6AEdZhQ53vVqrNwc/S6
Io6SPyQGdGV66i3Uq9Ltmp/IxGUjI7CD5BDTGGPxmNR27SJm4O5eBCGv56yVO+1MF8d8oE
09Uc8BdSk9ss1tHodSAcqXVq0MPgb4Lvb/dhGepIZzWPPCGa04MVf5lgediCcX+0V3PZSK
d5UfkRe4x6qYeSzWpIsXNAX/k0W8DiBLGQG2wy5PwVNzD1TZassJFqcRF4pNd54QJ4Pzg2
kybFWaFNy7ONDiooJ+8O/g/h7nNUK9xjjBMEdBsyodSyIiWsuBaxTtzYalbpZbiBSYiFga
wXtdxXzlcMw==
Decrypted message: Test Message!
---------------------- PERFORM XML TEST (END) ------------------------

$ CryptoTests.exe --test-pem --base $HOME/MinhasChaves
---------------------- PERFORM PEM TEST (BEGIN) ----------------------
Original message: Test Message!
Encrypted message: BSmrw1+UFNJkcgkJeMtPJ1JLra79fMDwxlINzeTSjQ2HEEoDAk0
GdGGQPpXGAhxL99VVDqxdXwDw/qn941GJ1HxOqo8acmRj2uqP6r//vqYLZa9mTF5n2DZfN
rpq2E7eCEKXazZkwRd5xaScNP3JnrilwJ5ZLqcHuAFfFwXgA3hlnNLSkuxtwXg1XceFsfm
WtcRHRBWh4QuA0El8VsQz+Kca9/fOdScw8dZcD/+0adZi3iuXL28jZ/BYrseonEYxh5KQr
P7CR5UBorNTi1eyzi78cbZg3/1x5bRQ6pyjjBTFMS5xM7Y4chUL7a4ShGi/KGJWvj7a2NQ
p40/DJMiBzw==
Decrypted message: Test Message!
---------------------- PERFORM PEM TEST (END) ------------------------
```

## CIFRANDO UMA MENSAGEM
```console
$ CryptoTests.exe --encrypt --message 'Minha mensagem'
Encrypted message: M7BC951zDHLcp1CG7GAlCS/x2GKtMnFL0pWWm2OKuBJKC8R6bne
ZqRnziU2l/fIboUCKUgtTZlCfmED1Ma3i3isoaHVfOdPU557IC9+zRLBvTRRN6Zkr7AV2k
I/IxzdDAZClX3mzfqlJ+bl1cm2GzNg23kITd8083WJ/CLXnfKuhQUZoJwMOdvM5FYFJsrP
PDm6xv5uHcPXBcd+K+AZ5eYV9B/cmGiJ1ctN7dSySMP4uLiuqOkz9vLpzYj+2RDgzpWOlM
lBvrGS2xYSgtUjJNChAtJ2qksuqB1VKpx4O/iSSueHt2yXs1fWeMtfn3ttJBA18WyOh4fA
a28+PbBTVpw==
```

## DECIFRANDO UMA MENSAGEM
```console
$ CryptoTests.exe --decrypt --message 'cbZg3/1x5...p40/DJMiBzw=='
Decrypted message: Minha mensagem
```

## CIFRANDO UMA MENSAGEM COM CHAVE PÚBLICA ESPECÍFICA
```console
$ CryptoTests.exe --encrypt --message 'Mensagem' --pubkey ./public.xml
$ CryptoTests.exe --encrypt --message 'Mensagem' --pubkey ./public.pem
Encrypted message: Xnbp8JNhQVY92/yYIr21Rry+vJVQvMPQROAxOQ6mQoFoT/4qiy/
XDn0icE7BoTtdzXVEzb+Tzmsp9XcUzprWTg4+cTHOW7oBA3OFKdhBvDlP+iCgdOisIVnbO
57QO6XDt1OfY0OlMEQzW5kp8N50Y/Z0i3pXXmdCC91lsRhZmhiyc5lRn85/K07yw0JpOB7
vL+FpraSxxhbB8/XkNaJRc028R9/J48eYz3FTeS7x9AMapEXSiSXhNjcQ4kG8tOxCwlt58
IzBn+VXfTo7U3+mFMzWZuTx140DTeH4KDzpTyRbV3ccGJm3ZyFFl3QSrJwI21WZwaQ/xn2
KQXoBl+qOsA==
```

## DECIFRANDO UMA MENSAGEM COM CHAVE PRIVADA ESPECÍFICA
```console
$ CryptoTests.exe -d --message 'cbZg...p4w==' --privkey ./private.xml
$ CryptoTests.exe -d --message 'cbZg...p4w==' --privkey ./private.pem
Decrypted message: Minha mensagem
```
