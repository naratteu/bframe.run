# bframe.run
blazor iframe runner for csx

## usage

```html
<iframe onload="import('https://naratteu.github.io/bframe.run/csx.mjs').then(m => m.init(this))">
    System.Console.WriteLine("Hello, World!");
</iframe>
```

임의 웹페이지에 어떤 전역적인 의존성없이도 컴포넌트처럼 갖다붙일 수 있습니다.

깃허브 페이지를 fork하는 등, 동일출처가 되도록 구성한다면 아래처럼도 사용 가능합니다.

```html
<iframe src="/bframe.run/csx.html">
    using System;
    Console.Write("Hello..");
    Console.WriteLine("World!");
    "csx result"
</iframe>
```

data uri 에 즉시 작동하는(!) 코드를 담아 공유할수도 있습니다.
```
data:text/html;charset=utf-8;base64,PGlmcmFtZSBvbmxvYWQ9ImltcG9ydCgnaHR0cHM6Ly9uYXJhdHRldS5naXRodWIuaW8vYmZyYW1lLnJ1bi9jc3gubWpzJykudGhlbihtID0+IG0uaW5pdCh0aGlzKSkiPlN5c3RlbS5Db25zb2xlLldyaXRlTGluZSgiSGVsbG8sIFdvcmxkISIpOyJIZWxsbywgQ3NoYXJwISI8L2lmcmFtZT4=
```

### local 배포예시

```bash
dotnet publish -o ./bin/pub && (
cd ./bin/pub
cat <<EOF > ./test.html
<iframe src="./wwwroot/csx.html">
    System.Console.WriteLine("Hello, World!");
    "Hello, Csharp!"
</iframe>
EOF
npx serve .)
```

### ipfs 배포예시

```bash
dotnet publish -o ./bin/pub && (
cd ./bin/pub
rm -r ./bframe.run
mv ./wwwroot ./bframe.run
cat <<EOF > ./test.html
<iframe src="./bframe.run/csx.html">
    System.Console.WriteLine("Hello, World!");
    "Hello, Csharp!"
</iframe>
EOF
ipfs add -r . | grep pub$ | awk '{print "https://ipfs.io/ipfs/" $2 "/test.html"}'
ipfs add -qr . | ipfs routing provide)
```
https://ipfs.io/ipfs/QmRSPovZ9e5FUBWgLAuMT8jhZcunQcHB8c613En4J3q7ZD/test.html

## todo

- web ide 내장? 연동?
    - 페이지를 재사용해 코드실행 가능하도록
- 로딩 시각화
- index.html 에 iframe 사용법 템플릿 자동완성
- csx 기능확장? 
    - 상태저장, [Formatting](https://www.nuget.org/packages/Microsoft.DotNet.Interactive.Formatting)
- .run/cs.html?
- Blazor 의존성 제거? dotnet webassembly로만?

## see also

- https://github.com/LostBeard/BlazorWASMScriptLoader
- https://github.com/jjonescz/DotNetLab
- https://github.com/dotnet/try/tree/main/src/Microsoft.TryDotNet.WasmRunner