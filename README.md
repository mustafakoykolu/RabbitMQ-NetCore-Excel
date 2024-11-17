# ExcelCreator
Dockerize edilmiş şekilde 2 adet servis bulundurur. UI projesinde login işlemi yapılır. Ardından excel oluşturmak için products sayfasına gidilir. Excel oluştur butonuna tıklandıktan sonra dosya db ye kaydedilir ve ayrıca rabbitMQ' ile worker service'e yeni dosyanın geldiği haber verilir.
Worker servis dosyayı işler ve yeni bir excel dosyası oluşturur.
Dosya oluşturulduktan sonra kullanıcı dosyayı UI üzerinden indirebilir.
