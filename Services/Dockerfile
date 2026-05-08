From microsoft/dotnet:8.0.0-sdk as build

ARG BuildCONFIG=RELEASE

ARG VERSION=1.0.0

COPY Tweetbook.csproj /build

RUN dotnet restore ./build/Tweetbook.csproj

COPY . ./bild/
WORKDIR /build/
RUN dotnet publish ./Tweetbook.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION


FROM microsoft/dotnet:8.0.0-aspnetcore-runtime
WORKDIR /app

COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "Tweetbook.dll"]